using Envirotrax.Common.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Envirotrax.Auth.Data.Models
{
    public class AppUser : IdentityUser<int>, IAspNetUserBase
    {
    }
}
