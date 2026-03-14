
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.Common.Data.Services.Definitions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.WaterSuppliers;

public class WaterSupplierService : Service<WaterSupplier, WaterSupplierDto>, IWaterSupplierService
{
    private readonly IWaterSupplierRepository _repository;
    private readonly ITenantProvidersService _tenantProvider;

    public WaterSupplierService(
        IMapper mapper,
        IWaterSupplierRepository repository,
        ITenantProvidersService tenantProvider)
        : base(mapper, repository)
    {
        _repository = repository;
        _tenantProvider = tenantProvider;
    }

    public Task<WaterSupplierDto> GetLoggedInSupplierAsync()
    {
        return GetLoggedInSupplierAsync(CancellationToken.None);
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
}