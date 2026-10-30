using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.TableSettings;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.TableSettings;

// There is no UserId: a table setting always belongs to the logged-in user, and the service sets it.
public class TableSettingDto : IDto
{
    public int Id { get; set; }

    public TableSettingType TableType { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    public string? Filters { get; set; }
}
