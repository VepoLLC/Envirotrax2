

using Envirotrax.Common.Data.Services.Definitions;
using Envirotrax.Common.Data.Services.Implementations;
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.Common.Domain.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Envirotrax.Common.Configuration;

public static class ServiceRegistrations
{
    public static IServiceCollection AddTenantProvider(this IServiceCollection services)
    {
        return services
            .AddHttpContextAccessor()
            .AddTransient<ITenantProvidersService, TenantProviderService>()
            .AddTransient<IAuthService, AuthService>();
    }

    public static IServiceCollection AddHtmlTemplateService(this IServiceCollection services, Action<HtmlTemplateOptions> templateConfigAction)
    {
        services
            .AddHttpContextAccessor()
            .Configure(templateConfigAction)
            .AddSingleton<IHtmlTemplateService, HtmlTemplateService>();

        return services;
    }

    public static IServiceCollection AddEmailService(this IServiceCollection services, IConfigurationSection emailConfigSection, Action<HtmlTemplateOptions> templateConfigAction)
    {
        services.AddHtmlTemplateService(templateConfigAction);

        services.Configure<EmailOptions>(emailConfigSection);
        services.AddTransient<IEmailService, EmailService>();

        return services;
    }

    public static IServiceCollection AddInternalApi<TOptions>(this IServiceCollection services, IConfigurationSection configuration)
        where TOptions : InternalApiOptions
    {
        return services
            .Configure<TOptions>(configuration)
            .AddTransient<IInternalApiClientService<TOptions>, InternalApiClientService<TOptions>>();
    }

    public static IServiceCollection AddInternalApi(this IServiceCollection services, IConfigurationSection configuration)
    {
        return services
            .Configure<InternalApiOptions>(configuration)
            .AddTransient<IInternalApiClientService, InternalApiClientService>();
    }
}