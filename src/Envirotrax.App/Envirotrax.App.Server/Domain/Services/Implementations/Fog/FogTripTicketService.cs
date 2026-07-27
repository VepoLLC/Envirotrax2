using System.Transactions;
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
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var deleted = await _repository.DeleteAsync(id);

        if (deleted == null || deleted.ProfessionalId != _authService.ProfessionalId || !string.IsNullOrEmpty(deleted.TransactionId))
        {
            return null;
        }

        scope.Complete();
        return MapToDto(deleted);
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
