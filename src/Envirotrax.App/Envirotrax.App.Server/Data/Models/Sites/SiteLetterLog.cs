using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Envirotrax.App.Server.Data.Models.Sites;

public class SiteLetterLog : TenantModel<WaterSupplier>
{
    public const int EmailPage = 0;

    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int SiteId { get; set; }
    public Site? Site { get; set; }

    public DateTime ProcessDate { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = null!;

    public int Page { get; set; }

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = null!;
}

public class SiteLetterLogConfiguration : IEntityTypeConfiguration<SiteLetterLog>
{
    public void Configure(EntityTypeBuilder<SiteLetterLog> builder)
    {
        builder.HasIndex(letterLog => letterLog.ProcessDate);
    }
}
