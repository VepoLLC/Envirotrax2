
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowGaugeService : Service<BackflowGauge, BackflowGaugeDto>, IBackflowGaugeService
{
    private readonly IBackflowGaugeRepository _gaugeRepository;

    public BackflowGaugeService(IMapper mapper, IBackflowGaugeRepository repository)
        : base(mapper, repository)
    {
        _gaugeRepository = repository;
    }

    public async Task<IPagedData<BackflowGaugeDto>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<BackflowGauge, BackflowGaugeDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<BackflowGauge, BackflowGaugeDto>(Mapper);

        var items = await _gaugeRepository.GetAllByProfessionalAsync(professionalId, pageInfo, query, cancellationToken);

        return items.Select(i => MapToDto(i)!).ToPagedData(pageInfo);
    }
}
