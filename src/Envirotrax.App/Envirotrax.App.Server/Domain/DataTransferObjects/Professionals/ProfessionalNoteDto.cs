using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

public class ProfessionalNoteDto : IDto
{
    public int Id { get; set; }

    [Required]
    public string NoteText { get; set; } = null!;

    // Audit
    public DateTime CreatedTime { get; set; }
    public AppUserDto? CreatedBy { get; set; }
}
