
using System.Transactions;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Csi;

public class CsiInspectorAccountService : Service<ProfessionalUser, CsiInspectorAccountDto>, ICsiInspectorAccountService
{
    // A professional can register with every water supplier in the system, and the details window shows
    // them all at once (the admin edits them in place), so the registrations are never paged.
    private const int MaxRegistrations = 2000;

    private readonly IProfessionalUserRepository _repository;
    private readonly IProfessionalService _professionalService;
    private readonly IProfessionalUserService _professionalUserService;
    private readonly IProfessionalSupplierService _supplierService;

    public CsiInspectorAccountService(
        IMapper mapper,
        IProfessionalUserRepository repository,
        IProfessionalService professionalService,
        IProfessionalUserService professionalUserService,
        IProfessionalSupplierService supplierService)
        : base(mapper, repository)
    {
        _repository = repository;
        _professionalService = professionalService;
        _professionalUserService = professionalUserService;
        _supplierService = supplierService;
    }

    public async Task<IPagedData<CsiInspectorAccountDto>> SearchForAdminAsync(PageInfo pageInfo, Query query, string? licenseNumber, string? insuranceNumber, CancellationToken cancellationToken)
    {
        query.Filter = query.ConvertFilterProperties<ProfessionalUser, CsiInspectorAccountDto>(Mapper);
        query.Sort = query.ConvertSortProperties<ProfessionalUser, CsiInspectorAccountDto>(Mapper);

        var accounts = await _repository.SearchCsiInspectorsAsync(pageInfo, query, licenseNumber, insuranceNumber, cancellationToken);

        return accounts
            .Select(a => MapToDto(a)!)
            .ToPagedData(pageInfo);
    }

    public async Task<CsiInspectorAccountDetailsDto?> GetDetailsForAdminAsync(int professionalId, int? userId, CancellationToken cancellationToken)
    {
        var professional = await _professionalService.GetAsync(professionalId, cancellationToken);

        if (professional == null)
        {
            return null;
        }

        var registrations = await GetRegistrationsAsync(professionalId, cancellationToken);
        var user = await GetUserAsync(professionalId, userId, cancellationToken);

        return new CsiInspectorAccountDetailsDto
        {
            Professional = professional,
            User = user,
            Registrations = [.. registrations]
        };
    }

    public async Task<CsiInspectorAccountDetailsDto?> UpdateDetailsForAdminAsync(int professionalId, CsiInspectorAccountDetailsDto details, CancellationToken cancellationToken)
    {
        var existing = await _professionalService.GetAsync(professionalId, cancellationToken);

        if (existing == null)
        {
            return null;
        }

        details.Professional.Id = professionalId;

        var currentRegistrations = await GetRegistrationsAsync(professionalId, cancellationToken);
        var changedRegistrations = GetChangedRegistrations(currentRegistrations, details.Registrations);

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            await _professionalService.UpdateAsync(details.Professional);

            if (details.User != null && details.User.Id > 0)
            {
                await _professionalUserService.UpdateSubAccountAsync(professionalId, details.User.Id, details.User.ContactName, details.User.JobTitle);
            }

            foreach (var registration in changedRegistrations)
            {
                registration.Professional = new ReferencedProfessionalDto
                {
                    Id = professionalId
                };

                await _supplierService.UpdateAsync(registration);
            }

            scope.Complete();
        }

        return await GetDetailsForAdminAsync(professionalId, details.User?.Id, cancellationToken);
    }

    /// <summary>
    /// The window posts every row back, but each one costs its own SaveChanges. Keep only the rows the
    /// admin actually edited, and drop anything the professional is not registered with so a crafted
    /// payload cannot attach a row that does not exist.
    /// </summary>
    private static List<ProfessionalWaterSupplierDto> GetChangedRegistrations(
        IEnumerable<ProfessionalWaterSupplierDto> current,
        IEnumerable<ProfessionalWaterSupplierDto> submitted)
    {
        var currentBySupplier = current.ToDictionary(r => r.WaterSupplier.Id!.Value);
        var changed = new List<ProfessionalWaterSupplierDto>();

        foreach (var registration in submitted)
        {
            var supplierId = registration.WaterSupplier?.Id;

            if (supplierId == null || !currentBySupplier.TryGetValue(supplierId.Value, out var original))
            {
                continue;
            }

            var isUnchanged =
                registration.HasCsiInspection == original.HasCsiInspection &&
                registration.IsBanned == original.IsBanned &&
                registration.CsiCommercialInspectionFee == original.CsiCommercialInspectionFee &&
                registration.CsiResidentialInspectionFee == original.CsiResidentialInspectionFee;

            if (!isUnchanged)
            {
                changed.Add(registration);
            }
        }

        return changed;
    }

    private async Task<IReadOnlyList<ProfessionalWaterSupplierDto>> GetRegistrationsAsync(int professionalId, CancellationToken cancellationToken)
    {
        var pageInfo = new PageInfo
        {
            PageNumber = 1,
            PageSize = MaxRegistrations
        };

        // V1 scoped this grid to CSI registrations (SaveWaterSupplierRegistrations.UserType = CsiInspector).
        // Filtering on HasCsiInpection alone would make the Active checkbox one-way - unticking it would
        // remove the row and leave no way back - so keep every supplier that runs a CSI program too.
        var registrations = await _supplierService.GetAllByProfessionalAsync(
            professionalId,
            pageInfo,
            new Query(),
            cancellationToken,
            proSupplier => proSupplier.HasCsiInpection || proSupplier.WaterSupplier!.GeneralSettings!.CsiInspections);

        return [.. registrations.Data.OrderBy(r => r.WaterSupplier?.Name)];
    }

    private async Task<ProfessionalUserDto?> GetUserAsync(int professionalId, int? userId, CancellationToken cancellationToken)
    {
        if (!userId.HasValue)
        {
            return null;
        }

        var users = await _professionalUserService.GetAllByProfessionalAsync(
            professionalId,
            new PageInfo(),
            new Query(),
            cancellationToken,
            proUser => proUser.UserId == userId.Value);

        return users.Data.FirstOrDefault();
    }
}
