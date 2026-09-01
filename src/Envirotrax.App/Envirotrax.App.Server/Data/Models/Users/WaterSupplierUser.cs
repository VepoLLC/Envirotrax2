
using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Envirotrax.App.Server.Data.Models.Users;

public class WaterSupplierUser : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(false)]
    public int UserId { get; set; }
    public AppUser? User { get; set; }

    [Required]
    [StringLength(100)]
    public string ContactName { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string EmailAddress { get; set; } = null!;

    public IEnumerable<UserRole>? UserRoles { get; set; }
}

public class WaterSupplierUserConfiguration : IEntityTypeConfiguration<WaterSupplierUser>
{
    public void Configure(EntityTypeBuilder<WaterSupplierUser> builder)
    {
        builder.HasMany(user => user.UserRoles)
            .WithOne()
            .HasForeignKey(userRole => new { userRole.WaterSupplierId, userRole.UserId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}