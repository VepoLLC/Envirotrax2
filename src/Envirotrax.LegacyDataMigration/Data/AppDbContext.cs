
using Envirotrax.LegacyDataMigration.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Envirotrax.LegacyDataMigration.Data;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{

}