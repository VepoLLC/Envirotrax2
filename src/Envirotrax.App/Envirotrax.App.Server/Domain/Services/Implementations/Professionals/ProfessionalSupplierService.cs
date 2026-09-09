
using System.Linq.Expressions;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class ProfessionalSupplierService : Service<ProfessionalWaterSupplier, ProfessionalWaterSupplierDto>, IProfessionalSupplierService
{
    private readonly IMapper _mapper;
    private readonly IProfessionalSupplierRepository _proSupplierRepository;
    private readonly IRecordLogService _recordLogService;

    public ProfessionalSupplierService(IMapper mapper, IProfessionalSupplierRepository repository, IRecordLogService recordLogService) : base(mapper, repository)
    {
        _mapper = mapper;
        _proSupplierRepository = repository;
        _recordLogService = recordLogService;
    }

    public override async Task<ProfessionalWaterSupplierDto?> DeleteAsync(int id)
    {
        var deleted = await base.DeleteAsync(id);

        if (deleted != null)
        {
            var waterSupplierId = deleted.WaterSupplier.Id ?? id;

            await _recordLogService.AddAsync(RecordLogTableNames.ProfessionalWaterSupplierRegistrations, waterSupplierId, waterSupplierId, RecordLogType.Delete,
                "Deleted water supplier registration", professionalId: deleted.Professional?.Id);
        }

        return deleted;
    }

    public async Task<IPagedData<ProfessionalWaterSupplierDto>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken, Expression<Func<ProfessionalWaterSupplier, bool>>? filter = null)
    {
        query.Sort = query.ConvertSortProperties<ProfessionalWaterSupplier, ProfessionalWaterSupplierDto>(_mapper);
        query.Filter = query.ConvertFilterProperties<ProfessionalWaterSupplier, ProfessionalWaterSupplierDto>(_mapper);

        var items = await _proSupplierRepository.GetAllByProfessionalAsync(professionalId, pageInfo, query, cancellationToken, filter);

        return items.Select(i => MapToDto(i)!).ToPagedData(pageInfo);
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
}