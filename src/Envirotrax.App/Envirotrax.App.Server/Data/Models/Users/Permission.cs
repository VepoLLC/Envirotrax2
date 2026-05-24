
using System.ComponentModel.DataAnnotations;
using Envirotrax.Common;
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.App.Server.Data.Models.Users;

public class Permission
{
    [AppPrimaryKey(false)]
    public PermissionType Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    public PermissionCategoryType Category { get; set; }
    public int? SortOrder { get; set; }

    // These properties tell whether this permission supports view, modify, and delete actions.
    // For example, water suppliers can view inspections and modify CSI inspections, but cannot delete them.
    // So it will not display the Modify and Delete checkboxes for that CSI Inspections permission.
    public bool CanView { get; set; }
    public bool CanModify { get; set; }
    public bool CanDelete { get; set; }
}

public enum PermissionCategoryType
{
    General = 1,
    Csi = 2,
    Backflow = 3,
    Fog = 4
}