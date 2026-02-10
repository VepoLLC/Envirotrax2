
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

    public bool HasWiseGuys { get; set; }
    public bool HasBackflowTesting { get; set; }
    public bool HasCsiInspection { get; set; }
    public bool HasFogInspection { get; set; }
    public bool HasFogTransportation { get; set; }
}