namespace Envirotrax.App.Server.Domain.Services.Definitions.Payments;

public interface IProfessionalReceiptService
{
    Task<byte[]?> GeneratePdfAsync(string transactionId, CancellationToken cancellationToken);
}
