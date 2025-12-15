

using Envirotrax.Common.Data.Services.Definitions;
using Envirotrax.Common.Data.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Envirotrax.Common.Configuration;

public static class ServiceRegistrations
{
    public static IServiceCollection AddTenantProvider(this IServiceCollection services)
    {
        return services
            .AddHttpContextAccessor()
            .AddTransient<ITenantProvidersService, TenantProviderService>();
    }
}