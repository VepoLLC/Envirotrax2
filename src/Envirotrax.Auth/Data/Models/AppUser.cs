using Envirotrax.Common.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Envirotrax.Auth.Data.Models
{
    public class AppUser : IdentityUser<int>, IAspNetUserBase
    {
        /// <summary>
        /// This property tells if the user is a Vepo admin and can access Envirotrax.Admin app
        /// </summary>
        public bool IsSuperUser { get; set; }
    }
}
