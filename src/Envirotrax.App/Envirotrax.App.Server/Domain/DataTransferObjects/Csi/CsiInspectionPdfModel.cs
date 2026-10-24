namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

public class CsiInspectionPdfModel
{
    public CsiInspectionDto Inspection { get; set; } = null!;

    public IReadOnlyList<CsiInspectionAssemblyDto> Assemblies { get; set; } = [];
}
