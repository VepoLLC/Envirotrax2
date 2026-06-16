
namespace Envirotrax.Admin.Server.Configuration;

public class AuthServerOptions
{
    public string Issuer { get; set; } = null!;
    public string? Audiences { get; set; }
}