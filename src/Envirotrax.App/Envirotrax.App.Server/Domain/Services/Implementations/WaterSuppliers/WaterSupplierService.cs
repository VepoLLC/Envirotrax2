
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.Common.Data.Services.Definitions;
using System.Transactions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.WaterSuppliers;

public class WaterSupplierService : Service<WaterSupplier, WaterSupplierDto>, IWaterSupplierService
{
    private readonly IWaterSupplierRepository _repository;
    private readonly ITenantProvidersService _tenantProvider;
    private readonly IGeneralSettingsService _generalSettingsService;
    private readonly IBackflowSettingsService _backflowSettingsService;

    public WaterSupplierService(
        IMapper mapper,
        IWaterSupplierRepository repository,
        ITenantProvidersService tenantProvider,
        IGeneralSettingsService generalSettingsService,
        IBackflowSettingsService backflowSettingsService)
        : base(mapper, repository)
    {
        _repository = repository;
        _tenantProvider = tenantProvider;
        _generalSettingsService = generalSettingsService;
        _backflowSettingsService = backflowSettingsService;
    }

    public Task<WaterSupplierDto> GetLoggedInSupplierAsync()
    {
        return GetLoggedInSupplierAsync(CancellationToken.None);
    }

    public Task<IEnumerable<int>> GetSupplierIdsAsync(bool hasBackflowTests, CancellationToken cancellationToken)
    {
        return _repository.GetSupplierIdsAsync(hasBackflowTests, cancellationToken);
    }

    public async Task<WaterSupplierDto> GetLoggedInSupplierAsync(CancellationToken cancellationToken)
    {
        var supplier = await _repository.GetNoIncludesAsync(_tenantProvider.WaterSupplierId, cancellationToken);
        return MapToDto(supplier) ?? throw new InvalidOperationException("User is not logged in to specific water supplier.");
    }

    private MySupplierHierarchyDto BuildHierarchy(IEnumerable<WaterSupplier> waterSuppliers)
    {
        var adminAccount = waterSuppliers
            .Where(s => s.ParentId == null)
            .Select(s => Mapper.Map<ReferencedWaterSupplierDto>(s))
            .FirstOrDefault();

        var supplierList = waterSuppliers
            .Where(s => s.ParentId.HasValue)
            .ToList();

        var idSet = supplierList.Select(s => (int?)s.Id).ToHashSet();

        var childrenByParentId = supplierList
            .Where(s => s.ParentId != null && idSet.Contains(s.ParentId))
            .GroupBy(s => s.ParentId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        var roots = supplierList
            .Where(s => adminAccount == null || s.ParentId == adminAccount?.Id)
            .Where(s => !idSet.Contains(s.ParentId));

        return new MySupplierHierarchyDto
        {
            AdminAccount = adminAccount,
            Hierarchy = GroupByLetter(roots, childrenByParentId)
        };
    }

    private IEnumerable<WaterSupplierHierarchyDto> GroupByLetter(IEnumerable<WaterSupplier> suppliers, Dictionary<int, List<WaterSupplier>> childrenByParentId)
    {
        return suppliers
            .Where(s => s.Domain != WaterSupplier.EnvirotraxAdminDomain)
            .GroupBy(s => s.Name?[..1].ToUpper() ?? "")
            .OrderBy(g => g.Key)
            .Select(g => new WaterSupplierHierarchyDto
            {
                GroupLetter = g.Key,
                WaterSuppliers = g
                    .OrderBy(s => s.Name)
                    .Select(s => new WaterSupplierHierarchyChildDto
                    {
                        WaterSupplier = Mapper.Map<ReferencedWaterSupplierDto>(s),
                        Children = childrenByParentId.TryGetValue(s.Id, out var children)
                            ? GroupByLetter(children, childrenByParentId)
                            : []
                    })
            });
    }

    public async Task<MySupplierHierarchyDto> GetAllMySuppliersAsync(CancellationToken cancellationToken)
    {
        var waterSuppliers = await _repository.GetAllMySuppliersAsync(cancellationToken);
        return BuildHierarchy(waterSuppliers);
    }

    public async Task<WaterSupplierDetailsDto?> GetDetailsAsync(int id, CancellationToken cancellationToken)
    {
        var supplier = await GetAsync(id, cancellationToken);

        if (supplier == null)
        {
            return null;
        }

        var generalSettings = await _generalSettingsService.GetAsync(id, cancellationToken);
        var backflowSettings = await _backflowSettingsService.GetAsync(id, cancellationToken);

        return new WaterSupplierDetailsDto
        {
            WaterSupplier = supplier,
            GeneralSettings = generalSettings ?? new GeneralSettingsDto { Id = id },
            BackflowSettings = backflowSettings ?? new BackflowSettingsDto { Id = id }
        };
    }

    public async Task<WaterSupplierDetailsDto?> UpdateDetailsAsync(int id, WaterSupplierDetailsDto details)
    {
        details.WaterSupplier.Id = id;

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var supplier = await _repository.UpdateAsync(MapToModel(details.WaterSupplier)!);

        if (supplier == null)
        {
            return null;
        }

        var generalSettings = await _generalSettingsService.AddOrUpdateAsync(id, details.GeneralSettings);
        var backflowSettings = await _backflowSettingsService.AddOrUpdateAsync(id, details.BackflowSettings);

        scope.Complete();

        return new WaterSupplierDetailsDto
        {
            WaterSupplier = MapToDto(supplier)!,
            GeneralSettings = generalSettings,
            BackflowSettings = backflowSettings
        };
    }
}