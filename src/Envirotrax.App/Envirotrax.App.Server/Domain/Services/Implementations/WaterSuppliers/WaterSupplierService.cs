
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Services.Implementations.WaterSuppliers;

public class WaterSupplierService : Service<WaterSupplier, WaterSupplierDto>, IWaterSupplierService
{
    private readonly IWaterSupplierRepository _repository;

    public WaterSupplierService(IMapper mapper, IWaterSupplierRepository repository)
        : base(mapper, repository)
    {
        _repository = repository;
    }

    public async Task<IPagedData<WaterSupplierDto>> GetAllMySuppliersAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<WaterSupplier, WaterSupplierContractor>(Mapper);
        query.Filter = query.ConvertFilterProperties<WaterSupplier, WaterSupplierDto>(Mapper);

        var supplierModels = await _repository.GetAllMySuppliersAsync(pageInfo, query, cancellationToken);
        var supplierDtos = Mapper.Map<IEnumerable<WaterSupplier>, IEnumerable<WaterSupplierDto>>(supplierModels);

        return supplierDtos.ToPagedData(pageInfo);
    }
}