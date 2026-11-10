using Envirotrax.Admin.Server.Domain.DataTransferObjects.Letters;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Letters;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Letters;

public class LetterProcessService : ILetterProcessService
{
    private const string BaseUrl = "/api/admin/letter-processes";

    private readonly IEnvirotraxApiClient _apiClient;

    public LetterProcessService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<List<LetterProcessDto>?> GetAllAsync(CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<List<LetterProcessDto>>(BaseUrl, cancellationToken);
    }

    public Task<List<LetterFileDto>?> GetFilesAsync(string processId, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<List<LetterFileDto>>($"{BaseUrl}/{Uri.EscapeDataString(processId)}/files", cancellationToken);
    }

    public Task<string?> GetFileUrlAsync(string processId, string fileName, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<string>($"{BaseUrl}/{Uri.EscapeDataString(processId)}/file-url?fileName={Uri.EscapeDataString(fileName)}", cancellationToken);
    }

    public Task DeleteAsync(string processId)
    {
        return _apiClient.DeleteAsync<object>($"{BaseUrl}/{Uri.EscapeDataString(processId)}", CancellationToken.None);
    }
}
