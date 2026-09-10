using Envirotrax.App.Server.Domain.DataTransferObjects.Api;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Api;

public interface ILegacySelectService
{
    Task<LegacySelectOutcome> ExecuteAsync(LegacySelectRequest request, CancellationToken cancellationToken);
}
