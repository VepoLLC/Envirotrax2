using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.Common.Domain.Services.Defintions;


namespace Envirotrax.App.Server.Domain.Services.Implementations.Csi;

public class CsiInspectionPdfService : ICsiInspectionPdfService
{
    private readonly IPdfTemplateService _pdfTemplateService;

    public CsiInspectionPdfService(IPdfTemplateService pdfTemplateService)
    {
        _pdfTemplateService = pdfTemplateService;
    }

    public Task<byte[]> GenerateAsync(CsiInspectionDto inspection)
    {
        return _pdfTemplateService.GenerateAsync("Csi.CsiInspection", inspection);
    }
}
