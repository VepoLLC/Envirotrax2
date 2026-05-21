
using System.ComponentModel.DataAnnotations;
using Envirotrax.Common;
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.App.Server.Data.Models.WaterSuppliers.Features;

public class Feature
{
    [AppPrimaryKey(false)]
    public FeatureType Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }
}