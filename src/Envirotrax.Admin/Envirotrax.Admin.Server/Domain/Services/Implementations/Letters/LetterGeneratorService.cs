using Envirotrax.Admin.Server.Domain.DataTransferObjects.Letters;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Letters;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Letters;

public class LetterGeneratorService : ILetterGeneratorService
{
    private readonly IEnvirotraxApiClient _apiClient;

    public LetterGeneratorService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<GeneratedLettersDto?> GenerateAsync(int waterSupplierId)
    {
        return _apiClient.PostAsync<object?, GeneratedLettersDto>(waterSupplierId, $"/api/admin/water-suppliers/{waterSupplierId}/letters", null, CancellationToken.None);
    }
}
