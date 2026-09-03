using System;

namespace Envirotrax.Auth.Domain.Security;

// Marks a Razor Page or controller as reachable even while the signed-in user carries the
// password_expired claim. Only the forced-change page itself and logout should ever use this.
[AttributeUsage(AttributeTargets.Class)]
public class AllowExpiredPasswordAttribute : Attribute
{
}
