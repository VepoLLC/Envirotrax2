using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Fog;

public interface IFogTripTicketService : IService<FogTripTicket, FogTripTicketDto>
{
    Task<IPagedData<FogTripTicketDto>> SearchForProfessionalAsync(PageInfo pageInfo, Query query, int? waterSupplierId, CancellationToken cancelationToken);

    Task<FogTripTicketDto> SubmitAsync(
        FogTripTicketDto request,
        Stream? generatorSignatureStream, string? generatorSignatureFileName,
        Stream? receiverSignatureStream, string? receiverSignatureFileName,
        CancellationToken cancellationToken);
}
