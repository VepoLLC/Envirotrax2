using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.Common.Data;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class SubmissionWaterSupplierService : ISubmissionWaterSupplierService
{
    private readonly IProfessionalSupplierRepository _supplierRepository;

    public SubmissionWaterSupplierService(IProfessionalSupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<int> ResolveAsync(SiteDto? site, int? requestedWaterSupplierId, ProfessionalType professionalType, CancellationToken cancellationToken)
    {
        var resolvedWaterSupplierId = site != null ? site.WaterSupplier?.Id : requestedWaterSupplierId;

        if (resolvedWaterSupplierId is not int waterSupplierId)
        {
            throw new AppValidationException("Please select a water supplier.");
        }

        await EnsureCanSubmitAsync(waterSupplierId, professionalType, cancellationToken);

        return waterSupplierId;
    }

    public async Task EnsureCanSubmitAsync(int waterSupplierId, ProfessionalType professionalType, CancellationToken cancellationToken)
    {
        var program = GetProgramName(professionalType);
        var registration = await _supplierRepository.GetRegistrationAsync(waterSupplierId, cancellationToken);

        if (registration == null || !IsRegistered(registration, professionalType))
        {
            var name = registration?.WaterSupplier?.Name ?? "this water supplier";

            throw new AppValidationException($"You are not registered for {program} with {name}. Add it to your water suppliers to continue.");
        }

        var supplier = registration.WaterSupplier!;

        if (IsSuspended(registration, professionalType))
        {
            throw new AppValidationException($"Your account is currently suspended from {program} with {supplier.Name}.");
        }

        if (!supplier.IsActive || !IsProgramEnabled(supplier, professionalType))
        {
            throw new AppValidationException($"{supplier.Name} is no longer active for {program}.");
        }
    }

    private static string GetProgramName(ProfessionalType professionalType)
    {
        return professionalType switch
        {
            ProfessionalType.Bpat => "backflow testing",
            ProfessionalType.CsiInspector => "CSI inspections",
            ProfessionalType.FogInspector => "FOG inspections",
            ProfessionalType.FogTransporter => "FOG transportation",
            _ => throw new ArgumentOutOfRangeException(nameof(professionalType))
        };
    }

    private static bool IsRegistered(ProfessionalWaterSupplier registration, ProfessionalType professionalType)
    {
        return professionalType switch
        {
            ProfessionalType.Bpat => registration.HasBackflowTesting,
            ProfessionalType.CsiInspector => registration.HasCsiInpection,
            ProfessionalType.FogInspector => registration.HasFogInspection,
            ProfessionalType.FogTransporter => registration.HasFogTransportation,
            _ => false
        };
    }

    private static bool IsSuspended(ProfessionalWaterSupplier registration, ProfessionalType professionalType)
    {
        return professionalType switch
        {
            ProfessionalType.Bpat => registration.IsBackflowTestingSuspended,
            ProfessionalType.CsiInspector => registration.IsCsiInspectionSuspended,
            ProfessionalType.FogInspector => registration.IsFogInspectionSuspended,
            ProfessionalType.FogTransporter => registration.IsFogTransportationSuspended,
            _ => false
        };
    }

    private static bool IsProgramEnabled(WaterSupplier supplier, ProfessionalType professionalType)
    {
        var settings = supplier.GeneralSettings;

        if (settings == null)
        {
            return false;
        }

        return professionalType switch
        {
            ProfessionalType.Bpat => settings.BackflowTesting,
            ProfessionalType.CsiInspector => settings.CsiInspections,
            ProfessionalType.FogInspector or ProfessionalType.FogTransporter => settings.FogProgram,
            _ => false
        };
    }
}
