using Envirotrax.App.Server.Data.Models.Api;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Api;

public interface IApiAuthenticationService
{
    Task<ApiAccount?> AuthenticateAsync(string? userId, string? apiKey, CancellationToken cancellationToken);
}
