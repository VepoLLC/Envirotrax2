
using Envirotrax.LegacyDataMigration.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.LegacyDataMigration.Data;

public class AppIdentityDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
        : base(options)
    {
    }
}
