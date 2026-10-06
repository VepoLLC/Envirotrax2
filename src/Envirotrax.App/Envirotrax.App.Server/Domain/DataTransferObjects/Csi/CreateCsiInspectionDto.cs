namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

// The inspection form in one request: the inspection plus its "Assemblies at this Location" tab.
public class CreateCsiInspectionDto : CsiInspectionDto
{
    // The rows the inspector kept: a row already saved on this inspection (Id, when editing) or a
    // current test at the site that is not on the inspection yet (TestId).
    public List<CsiInspectionAssemblySelectionDto> Assemblies { get; set; } = [];

    public List<CsiInspectionNewAssemblyDto> NewAssemblies { get; set; } = [];
}

public class CsiInspectionAssemblySelectionDto
{
    public int? Id { get; set; }

    public int? TestId { get; set; }

    public bool VisuallyIdentified { get; set; }
}
