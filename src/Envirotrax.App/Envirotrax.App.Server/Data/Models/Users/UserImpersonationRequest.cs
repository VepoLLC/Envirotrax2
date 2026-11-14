
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Envirotrax.App.Server.Data.Models.Users;

public class UserImpersonationRequest : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int SourceUserId { get; set; }
    public AppUser? SourceUser { get; set; }

    public int TargetUserId { get; set; }
    public AppUser? TargetUser { get; set; }

    public DateTime CreatedTime { get; set; }
}

public class UserImpersonationRequestConfiguration : IEntityTypeConfiguration<UserImpersonationRequest>
{
    public void Configure(EntityTypeBuilder<UserImpersonationRequest> builder)
    {
        builder.HasOne(request => request.SourceUser)
            .WithMany()
            .HasForeignKey(request => request.SourceUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(request => request.TargetUser)
            .WithMany()
            .HasForeignKey(request => request.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
