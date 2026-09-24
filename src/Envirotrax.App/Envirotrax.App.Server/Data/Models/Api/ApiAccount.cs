using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Envirotrax.App.Server.Data.Models.Api;

[Table("ApiAccounts")]
public class ApiAccount : ICreateAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string UserId { get; set; } = null!;

    [Required]
    [StringLength(64)]
    public string ApiKeyHash { get; set; } = null!;

    public int WaterSupplierId { get; set; }

    public bool IsLive { get; set; }

    public ApiPermissionLevel PermissionWaterSuppliers { get; set; }
    public ApiPermissionLevel PermissionSites { get; set; }
    public ApiPermissionLevel PermissionBackflowTests { get; set; }
    public ApiPermissionLevel PermissionCsiInspections { get; set; }
    public ApiPermissionLevel PermissionFogInspections { get; set; }
    public ApiPermissionLevel PermissionFogTripTickets { get; set; }

    // Audit properties
    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
}

public class ApiAccountConfiguration : IEntityTypeConfiguration<ApiAccount>
{
    public void Configure(EntityTypeBuilder<ApiAccount> builder)
    {
        builder.Property(account => account.CreatedTime)
            .HasDefaultValueSql("getdate()");

        builder.Property(account => account.WaterSupplierId)
            .HasDefaultValue(0);

        builder.Property(account => account.IsLive)
            .HasDefaultValue(false);

        builder.Property(account => account.PermissionWaterSuppliers)
            .HasDefaultValue(ApiPermissionLevel.Deny);

        builder.Property(account => account.PermissionSites)
            .HasDefaultValue(ApiPermissionLevel.Deny);

        builder.Property(account => account.PermissionBackflowTests)
            .HasDefaultValue(ApiPermissionLevel.Deny);

        builder.Property(account => account.PermissionCsiInspections)
            .HasDefaultValue(ApiPermissionLevel.Deny);

        builder.Property(account => account.PermissionFogInspections)
            .HasDefaultValue(ApiPermissionLevel.Deny);

        builder.Property(account => account.PermissionFogTripTickets)
            .HasDefaultValue(ApiPermissionLevel.Deny);
    }
}
