
using System.ComponentModel.DataAnnotations;
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.Auth.Data.Models;

[ExcludedModel]
[ReadOnlyModel]
public class Professional
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    public int? ParentId { get; set; }
    public Professional? Parent { get; set; }
}