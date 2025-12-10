
using System.ComponentModel.DataAnnotations;
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.App.Server.Data.Models.Contractors;

public class Contractor
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = null!;
}