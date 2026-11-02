using Envirotrax.App.Server.Domain.DataTransferObjects;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Helpers;

public class PdfLinkService : IPdfLinkService
{
    private const string TempFolder = "temp-pdf";

    private readonly IFileStorageService _fileStorageService;

    public PdfLinkService(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    public async Task<PdfLinkDto> CreateAsync(byte[] pdf, string fileName)
    {
        using var stream = new MemoryStream(pdf);

        var blobName = await _fileStorageService.UploadAsync($"{TempFolder}/{Guid.NewGuid():N}/{fileName}", stream);
        var url = await _fileStorageService.GenerateSasUrlAsync(blobName);

        return new PdfLinkDto
        {
            Url = url.ToString()
        };
    }
}
