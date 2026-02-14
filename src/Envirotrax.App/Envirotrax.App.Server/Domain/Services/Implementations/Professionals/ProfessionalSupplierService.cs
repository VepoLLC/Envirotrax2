
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class ProfessionalSupplierService : IProfessionalSupplierService
{
    private readonly IMapper _mapper;
    private readonly IProfessionalSupplierRepository _proSupplierRepository;

    public ProfessionalSupplierService(IMapper mapper, IProfessionalSupplierRepository proSupplierRepository)
    {
        _mapper = mapper;
        _proSupplierRepository = proSupplierRepository;
    }

    public async Task<IPagedData<AvailableWaterSupplierDto>> GetAllAvailableSuppliersAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<AvailableWaterSupplier, AvailableWaterSupplierDto>(_mapper);
        query.Filter = query.ConvertFilterProperties<AvailableWaterSupplier, AvailableWaterSupplierDto>(_mapper);

        var suppliers = await _proSupplierRepository.GetAllAvailableSuppliersAsync(pageInfo, query, cancellationToken);

        return _mapper
            .Map<IEnumerable<AvailableWaterSupplier>, IEnumerable<AvailableWaterSupplierDto>>(suppliers)
            .ToPagedData(pageInfo);
    }

    public async Task<IEnumerable<ProfessionalWaterSupplierDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var proSuppliers = await _proSupplierRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ProfessionalWaterSupplier>, IEnumerable<ProfessionalWaterSupplierDto>>(proSuppliers);
    }

    public async Task<ProfessionalWaterSupplierDto> AddOrUpdateAsync(ProfessionalWaterSupplierDto proSupplier)
    {
        var model = _mapper.Map<ProfessionalWaterSupplierDto, ProfessionalWaterSupplier>(proSupplier);

        var added = await _proSupplierRepository.AddOrUpdateAsync(model);

        return _mapper.Map<ProfessionalWaterSupplier, ProfessionalWaterSupplierDto>(added);
    }
}