
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

    public async Task<IPagedData<WaterSupplierDto>> GetAllMySuppliersAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<WaterSupplier, WaterSupplierProfessional>(Mapper);
        query.Filter = query.ConvertFilterProperties<WaterSupplier, WaterSupplierDto>(Mapper);

        var supplierModels = await _repository.GetAllMySuppliersAsync(pageInfo, query, cancellationToken);
        var supplierDtos = Mapper.Map<IEnumerable<WaterSupplier>, IEnumerable<WaterSupplierDto>>(supplierModels);

        return supplierDtos.ToPagedData(pageInfo);
    }
}