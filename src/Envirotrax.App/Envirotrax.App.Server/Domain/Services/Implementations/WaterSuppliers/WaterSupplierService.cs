
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

    private IEnumerable<WaterSupplierHierarchyDto> BuildHierarchy(IEnumerable<WaterSupplier> waterSuppliers)
    {
        var supplierList = waterSuppliers.ToList();
        var idSet = supplierList.Select(s => (int?)s.Id).ToHashSet();

        var childrenByParentId = supplierList
            .Where(s => s.ParentId != null && idSet.Contains(s.ParentId))
            .GroupBy(s => s.ParentId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        var roots = supplierList
            .Where(s => s.ParentId == null || !idSet.Contains(s.ParentId));

        return roots
            .Select(root => new WaterSupplierHierarchyDto
            {
                GroupLetter = root.Name?[..1].ToUpper(),
                WaterSupplier = Mapper.Map<ReferencedWaterSupplierDto>(root),
                Children = childrenByParentId.TryGetValue(root.Id, out var children)
                    ? Mapper.Map<IEnumerable<ReferencedWaterSupplierDto>>(children)
                    : []
            })
            .OrderBy(h => h.GroupLetter)
            .ThenBy(h => h.WaterSupplier.Name);
    }

    public async Task<IEnumerable<WaterSupplierHierarchyDto>> GetAllMySuppliersAsync(CancellationToken cancellationToken)
    {
        var waterSuppliers = await _repository.GetAllMySuppliersAsync(cancellationToken);
        return BuildHierarchy(waterSuppliers);
    }
}