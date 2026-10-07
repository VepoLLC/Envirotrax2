using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Fog;

public interface IFogTripTicketService : IService<FogTripTicket, FogTripTicketDto>
{
    Task<InsuranceCheckDto> GetInsuranceCheckAsync(int waterSupplierId, CancellationToken cancellationToken);

    Task<IPagedData<FogTripTicketDto>> SearchForProfessionalAsync(PageInfo pageInfo, Query query, int? waterSupplierId, CancellationToken cancelationToken);

    Task<FogTripTicketDto?> GetForProfessionalAsync(int id, CancellationToken cancellationToken);

    Task<FogTripTicketDto> SubmitAsync(
        FogTripTicketDto request,
        Stream? generatorSignatureStream, string? generatorSignatureFileName,
        Stream? receiverSignatureStream, string? receiverSignatureFileName,
        CancellationToken cancellationToken);

    Task<FogTripTicketDto?> UpdateApprovalAsync(int id, bool disapproved, CancellationToken cancellationToken);

    Task<byte[]> GeneratePdfAsync(FogTripTicketDto ticket);

    Task<byte[]> GeneratePdfAsync(IEnumerable<FogTripTicketDto> tickets);

    Task<byte[]> GeneratePdfWithSignaturesAsync(List<FogTripTicketDto> tickets);

    Task<byte[]> GeneratePdfForProfessionalAsync(FogTripTicketDto ticket);
}
