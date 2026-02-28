
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Users;

public class ReferencedPermissionDto
{
    [Required]
    public PermissionType? Id { get; set; }

    public string? Name { get; set; }

    public string? Category { get; set; }

    public int SortOrder { get; set; }

    // These properties tell whether this permission supports view, create, edit, and delete actions.
    // For example, water suppliers can view inpections and edit CSI inspections, but cannot create or delete them
    // So it will not display the Create and Delete checkboxes for that CSI Inpsections permission
    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
}
