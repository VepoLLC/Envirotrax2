namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

/// <summary>
/// Slim set of backflow-testing display flags used to gate the
/// "Additional Information" section on the professional side.
/// </summary>
public class BackflowTestingSettingsDto
{
    public bool ShowRainSensor { get; set; }
    public bool ShowOSSF { get; set; }
    public bool ShowPermitNumber { get; set; }
}
