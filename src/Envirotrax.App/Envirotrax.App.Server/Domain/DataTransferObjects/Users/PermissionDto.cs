
using System.ComponentModel.DataAnnotations;
using Envirotrax.Common;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Users;

public class ReferencedPermissionDto
{
    [Required]
    public PermissionType? Id { get; set; }

    public string? Name { get; set; }

    public string? Category { get; set; }

    public int SortOrder { get; set; }

    // These properties tell whether this permission supports view, modify, and delete actions.
    // For example, water suppliers can view inspections and modify CSI inspections, but cannot delete them.
    // So it will not display the Modify and Delete checkboxes for that CSI Inspections permission.
    public bool CanView { get; set; }
    public bool CanModify { get; set; }
    public bool CanDelete { get; set; }
}
