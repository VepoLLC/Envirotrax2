using Envirotrax.App.Server.Domain.DataTransferObjects;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Helpers;

public interface IPdfViewService
{
    PdfViewDto Store(byte[] content, string fileName);

    PdfViewDto? Find(string token);
}
