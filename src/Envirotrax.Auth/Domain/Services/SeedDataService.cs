using Envirotrax.Auth.Data;
using Envirotrax.Auth.Domain.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;

namespace Envirotrax.Auth.Areas.OpenIdConnect.Services;

public class SeedDataService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly OpenIddictOptions _openIddictOptions;

    public SeedDataService(IServiceProvider serviceProvider, IOptions<OpenIddictOptions> openIddictOptions)
    {
        _serviceProvider = serviceProvider;
        _openIddictOptions = openIddictOptions.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync(cancellationToken);

            await AddApiClientsAsync(scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>());
            await AddApiResources(scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>());
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task AddApiResources(IOpenIddictScopeManager scopeManager)
    {
        if (_openIddictOptions.Resources != null)
        {
            foreach (var resource in _openIddictOptions.Resources)
            {
                if (await scopeManager.FindByNameAsync(resource.Value.Name) == null)
                {
                    var openIddictResource = new OpenIddictScopeDescriptor
                    {
                        Name = resource.Value.Name,
                        DisplayName = resource.Value.DisplayName,
                    };

                    await scopeManager.CreateAsync(openIddictResource);

                    foreach (var scope in resource.Value.AllowedScopes.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    {
                        openIddictResource.Resources.Add(scope);
                    }
                }
            }
        }
    }

    private async Task AddApiClientsAsync(IOpenIddictApplicationManager openIddictManager)
    {
        foreach (var client in _openIddictOptions.Clients)
        {
            if (await openIddictManager.FindByClientIdAsync(client.Value.ClientId) == null)
            {
                var openIddictApp = new OpenIddictApplicationDescriptor()
                {
                    ClientId = client.Value.ClientId,
                    ClientSecret = client.Value.ClientSecret,
                    Permissions =
                        {
                            OpenIddictConstants.Permissions.Endpoints.Token
                        },
                    ClientType = client.Value.IsPublic
                        ? OpenIddictConstants.ClientTypes.Public
                        : OpenIddictConstants.ClientTypes.Confidential
                };

                var grantTypes = client.Value.GrantTypes.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach (var permission in grantTypes)
                {
                    if (permission == "authorization_code")
                    {
                        openIddictApp.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
                        openIddictApp.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
                        openIddictApp.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
                        openIddictApp.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Email);
                        openIddictApp.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Profile);
                        openIddictApp.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Roles);
                        openIddictApp.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.EndSession);
                    }

                    openIddictApp.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.GrantType + permission);
                }

                var scopes = client.Value.Scopes.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(s => OpenIddictConstants.Permissions.Prefixes.Scope + s);

                foreach (var scope in scopes)
                {
                    openIddictApp.Permissions.Add(scope);
                }

                if (client.Value.RedirectUris != null)
                {
                    var redirectUris = client.Value.RedirectUris.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    foreach (var redirectUri in redirectUris)
                    {
                        openIddictApp.RedirectUris.Add(new Uri(redirectUri));
                    }
                }

                if (client.Value.LogoutUris != null)
                {
                    var logoutUris = client.Value.LogoutUris.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries); ;

                    foreach (var logoutUri in logoutUris)
                    {
                        openIddictApp.PostLogoutRedirectUris.Add(new Uri(logoutUri));
                    }
                }

                await openIddictManager.CreateAsync(openIddictApp);
            }
        }
    }
}