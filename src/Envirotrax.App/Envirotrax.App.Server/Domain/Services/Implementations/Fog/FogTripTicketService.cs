using System.ComponentModel.DataAnnotations;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogTripTicketService : Service<FogTripTicket, FogTripTicketDto>, IFogTripTicketService
{
    private readonly IFogTripTicketRepository _repository;
    private readonly IAuthService _authService;

    public FogTripTicketService(IMapper mapper, IFogTripTicketRepository repository, IAuthService authService)
        : base(mapper, repository)
    {
        _repository = repository;
        _authService = authService;
    }

    public override async Task<FogTripTicketDto?> DeleteAsync(int id)
    {
        var ticket = await _repository.GetNoIncludesAsync(id, CancellationToken.None);

        if (ticket == null || ticket.ProfessionalId != _authService.ProfessionalId || !string.IsNullOrEmpty(ticket.TransactionId))
        {
            return null;
        }

        return await base.DeleteAsync(id);
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
