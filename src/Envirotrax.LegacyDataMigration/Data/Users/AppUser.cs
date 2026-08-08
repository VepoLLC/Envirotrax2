
using Microsoft.AspNetCore.Identity;

namespace Envirotrax.LegacyDataMigration.Data.Users;

public class AppUser : IdentityUser<int>
{
    public bool IsMigratedLegacyPasswordHashed { get; set; }

    public DateTime? PasswordExpirationDate { get; set; }
}