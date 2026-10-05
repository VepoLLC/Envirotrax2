using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

// The checked rows of the form's Assemblies tab; every other row of the submission is unchecked.
public class CsiInspectionVisuallyIdentifiedRequest
{
    [Required]
    [MaxLength(50)]
    public string SubmissionId { get; set; } = null!;

    public List<int> VisuallyIdentifiedIds { get; set; } = [];
}
