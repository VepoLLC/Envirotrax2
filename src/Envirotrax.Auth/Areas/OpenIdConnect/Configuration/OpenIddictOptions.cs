
namespace Envirotrax.Auth.Areas.OpenIdConnect.Configuration;

public class OpenIddictOptions
{
    public Dictionary<string, OpenIddictClient> Clients { get; set; } = null!;
    public Dictionary<string, OpenIddictResource>? Resources { get; set; }
}

public class OpenIddictClient
{
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string Scopes { get; set; } = null!;
    public string GrantTypes { get; set; } = null!;
    public string? RedirectUris { get; set; }
    public string? LogoutUris { get; set; }
    public bool IsPublic { get; set; }
}

public class OpenIddictResource
{
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string AllowedScopes { get; set; } = null!;
}