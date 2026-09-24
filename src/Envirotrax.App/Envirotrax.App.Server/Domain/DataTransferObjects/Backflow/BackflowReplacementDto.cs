namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowReplacementDto : BackflowTestDto
{
    public bool ValidationReplacementOnHold { get; set; }

    public bool ValidationReplacementCleared { get; set; }
}
