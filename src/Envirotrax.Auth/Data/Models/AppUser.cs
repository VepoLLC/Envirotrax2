using Envirotrax.Common.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Envirotrax.Auth.Data.Models
{
    public class AppUser : IdentityUser<int>, IAspNetUserBase
    {
        public AppUser()
        {
            // MFA is mandatory by default; only a direct database change should ever turn it off for a user.
            TwoFactorEnabled = true;
        }

        /// <summary>
        /// This property tells if the user is a Vepo admin and can access Envirotrax.Admin app
        /// </summary>
        public bool IsSuperUser { get; set; }

        public DateTime? PasswordExpirationDate { get; set; }

        /// <summary>
        /// True only once the user has verified a code from their authenticator app. Unlike
        /// TwoFactorEnabled (always true) or the presence of an authenticator key (which
        /// ResetAuthenticatorKeyAsync regenerates immediately, before the new key is verified),
        /// this is the actual signal for whether the authenticator method is usable at login.
        /// </summary>
        public bool AuthenticatorConfirmed { get; set; }
    }
}
