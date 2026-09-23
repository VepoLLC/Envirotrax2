namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowTestAdminDetailsDto : BackflowTestDto
{
    public string? BpatJobTitle { get; set; }

    public bool ValidationNewSite { get; set; }
    public bool ValidationSiteInformationChanged { get; set; }
    public bool ValidationUnknownSerialNumber { get; set; }
    public bool ValidationDeviceInformationChanged { get; set; }
    public string? ValidationNotes { get; set; }

    public bool ShowRainSensor { get; set; }
    public bool ShowOSSF { get; set; }
    public bool ShowPermitNumber { get; set; }
}
