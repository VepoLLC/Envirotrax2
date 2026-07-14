using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogTripTicketService : Service<FogTripTicket, FogTripTicketDto>, IFogTripTicketService
{
    private readonly IFogTripTicketRepository _repository;

    public FogTripTicketService(IMapper mapper, IFogTripTicketRepository repository)
        : base(mapper, repository)
    {
        _repository = repository;
    }

    public async Task<IPagedData<FogTripTicketDto>> SearchForProfessionalAsync(
        PageInfo pageInfo, Query query, int? waterSupplierId, CancellationToken cancelationToken)
    {
        query.Filter = query.ConvertFilterProperties<FogTripTicket, FogTripTicketDto>(Mapper);
        query.Sort = query.ConvertSortProperties<FogTripTicket, FogTripTicketDto>(Mapper);

        var tickets = await _repository.SearchForProfessionalAsync(pageInfo, query, waterSupplierId, cancelationToken);

        return tickets.Select(Mapper.Map<FogTripTicketDto>).ToPagedData(pageInfo);
    }
}
