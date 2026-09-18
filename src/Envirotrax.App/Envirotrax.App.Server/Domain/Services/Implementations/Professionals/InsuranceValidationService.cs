using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.Common.Data;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

/// <summary>
/// Single source of truth for insurance eligibility, ported from V1's WaterSupplierLicensing.CheckInsurance.
/// The submission screens read it through /api/professionals/insurance-status; the submit services enforce
/// it through EnsureValidAsync.
/// </summary>
public class InsuranceValidationService : IInsuranceValidationService
{
    private readonly IGeneralSettingsService _generalSettingsService;
    private readonly IProfessionalInsuranceRepository _insuranceRepository;
    private readonly IProfessionalRepository _professionalRepository;
    private readonly ITimeZoneHelperService _timeZoneHelper;
    private readonly IAuthService _authService;

    public InsuranceValidationService(
        IGeneralSettingsService generalSettingsService,
        IProfessionalInsuranceRepository insuranceRepository,
        IProfessionalRepository professionalRepository,
        ITimeZoneHelperService timeZoneHelper,
        IAuthService authService)
    {
        _generalSettingsService = generalSettingsService;
        _insuranceRepository = insuranceRepository;
        _professionalRepository = professionalRepository;
        _timeZoneHelper = timeZoneHelper;
        _authService = authService;
    }

    public async Task<InsuranceValidationDto> ValidateAsync(int waterSupplierId, ProfessionalType professionalType, CancellationToken cancellationToken)
    {
        var settings = await _generalSettingsService.GetAsync(waterSupplierId, cancellationToken);
        var (requires, requiredCoverage) = GetRequirement(settings, professionalType);

        if (!requires)
        {
            return new InsuranceValidationDto { Status = InsuranceStatus.NotRequired, RequiredCoverage = 0 };
        }

        var professionalId = _authService.ProfessionalId;
        var status = await EvaluatePoliciesAsync(professionalId, requiredCoverage, cancellationToken);

        if (status != InsuranceStatus.Valid)
        {
            // A sub-account may fall back to its master account's policy, as in V1. Its own policy is
            // checked first, so this is never stricter than V1.
            var professional = await _professionalRepository.GetAsync(professionalId, cancellationToken);

            if (professional?.ParentId != null)
            {
                var parentStatus = await EvaluatePoliciesAsync(professional.ParentId.Value, requiredCoverage, cancellationToken);

                status = parentStatus == InsuranceStatus.Valid
                    ? InsuranceStatus.Valid
                    : PreferMoreActionable(status, parentStatus);
            }
        }

        return new InsuranceValidationDto { Status = status, RequiredCoverage = requiredCoverage };
    }

    public async Task EnsureValidAsync(int waterSupplierId, ProfessionalType professionalType, CancellationToken cancellationToken)
    {
        var validation = await ValidateAsync(waterSupplierId, professionalType, cancellationToken);

        if (validation.Status is InsuranceStatus.Valid or InsuranceStatus.NotRequired)
        {
            return;
        }

        throw new AppValidationException(BuildMessage(validation));
    }

    /// <summary>
    /// A professional may hold several policies. If none qualify, the most actionable failure is returned
    /// so the contractor sees what is actually blocking them rather than an unrelated expired policy.
    /// </summary>
    private async Task<InsuranceStatus> EvaluatePoliciesAsync(int professionalId, decimal requiredCoverage, CancellationToken cancellationToken)
    {
        var policies = await _insuranceRepository.GetAllForValidationAsync(professionalId, cancellationToken);

        if (policies.Count == 0)
        {
            return InsuranceStatus.NotFound;
        }

        var now = _timeZoneHelper.GetUserLocalTime();
        var best = InsuranceStatus.NotFound;

        foreach (var policy in policies)
        {
            InsuranceStatus current;

            if (policy.ExpirationDate == null)
            {
                current = InsuranceStatus.AwaitingValidation;
            }
            else if (policy.ExpirationDate <= now)
            {
                current = InsuranceStatus.Expired;
            }
            else if ((policy.InsuranceCoverage ?? 0m) < requiredCoverage)
            {
                current = InsuranceStatus.InsufficientCoverage;
            }
            else
            {
                return InsuranceStatus.Valid;
            }

            best = PreferMoreActionable(best, current);
        }

        return best;
    }

    private static InsuranceStatus PreferMoreActionable(InsuranceStatus a, InsuranceStatus b)
    {
        return Rank(b) > Rank(a) ? b : a;
    }

    private static int Rank(InsuranceStatus status)
    {
        return status switch
        {
            InsuranceStatus.InsufficientCoverage => 3,
            InsuranceStatus.AwaitingValidation => 2,
            InsuranceStatus.Expired => 1,
            InsuranceStatus.NotFound => 0,
            _ => -1
        };
    }

    private static (bool Requires, decimal RequiredCoverage) GetRequirement(GeneralSettingsDto? settings, ProfessionalType professionalType)
    {
        // FOG inspectors and everything else: no requirement, matching V1 where that check was disabled.
        return professionalType switch
        {
            ProfessionalType.Bpat => (settings?.BpatsRequireInsurance ?? false, settings?.BpatsRequireInsuranceAmount ?? 0m),
            ProfessionalType.CsiInspector => (settings?.CsiInspectorsRequireInsurance ?? false, settings?.CsiInspectorsRequireInsuranceAmount ?? 0m),
            ProfessionalType.FogTransporter => (settings?.FogTransportersRequireInsurance ?? false, settings?.FogTransportersRequireInsuranceAmount ?? 0m),
            _ => (false, 0m)
        };
    }

    private static string BuildMessage(InsuranceValidationDto validation)
    {
        return validation.Status switch
        {
            InsuranceStatus.NotFound => "No insurance policy found.",
            InsuranceStatus.AwaitingValidation => "Insurance policy is awaiting validation.",
            InsuranceStatus.Expired => "Insurance policy has expired.",
            InsuranceStatus.InsufficientCoverage => $"Insurance policy requires ${validation.RequiredCoverage:#,0} in coverage.",
            _ => "Insurance policy is not valid."
        };
    }
}
