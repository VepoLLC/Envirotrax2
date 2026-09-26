using Envirotrax.App.Server.Data.Models.Api;
using Envirotrax.App.Server.Data.Repositories.Definitions.Api;
using Envirotrax.App.Server.Domain.Services.Definitions.Api;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Api;

public class ApiAuthenticationService : IApiAuthenticationService
{
    private readonly IApiAccountRepository _apiAccountRepository;
    private readonly IKeyHashingService _keyHashingService;

    public ApiAuthenticationService(
        IApiAccountRepository apiAccountRepository,
        IKeyHashingService keyHashingService)
    {
        _apiAccountRepository = apiAccountRepository;
        _keyHashingService = keyHashingService;
    }

    public async Task<ApiAccount?> AuthenticateAsync(string? userId, string? apiKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(apiKey))
        {
            return null;
        }

        var apiKeyHash = _keyHashingService.HashText(apiKey);

        return await _apiAccountRepository.GetByCredentialsAsync(userId, apiKeyHash, cancellationToken);
    }
}
