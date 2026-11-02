using Envirotrax.App.Server.Domain.DataTransferObjects;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Helpers;

public interface IPdfLinkService
{
    Task<PdfLinkDto> CreateAsync(byte[] pdf, string fileName);
}
