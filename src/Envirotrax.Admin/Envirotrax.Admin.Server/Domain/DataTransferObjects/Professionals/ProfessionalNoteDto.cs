using System.ComponentModel.DataAnnotations;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;

public class ProfessionalNoteDto
{
    public int Id { get; set; }

    [Required]
    public string NoteText { get; set; } = null!;

    // Audit
    public DateTime CreatedTime { get; set; }
    public ProfessionalNoteCreatedByDto? CreatedBy { get; set; }
}

public class ProfessionalNoteCreatedByDto
{
    public int Id { get; set; }

    public string? Email { get; set; }
}
