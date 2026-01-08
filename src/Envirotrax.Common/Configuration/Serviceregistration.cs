

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
            .AddTransient<ITenantProvidersService, TenantProviderService>();
    }

    public static IServiceCollection AddHtmlTemplateService(this IServiceCollection services, Action<HtmlTemplateOptions> templateConfigAction)
    {
        services.Configure(templateConfigAction);
        services.AddSingleton<IHtmlTemplateService, HtmlTemplateService>();

        return services;
    }

    public static IServiceCollection AddEmailService(this IServiceCollection services, IConfigurationSection emailConfigSection, Action<HtmlTemplateOptions> templateConfigAction)
    {
        services.AddHtmlTemplateService(templateConfigAction);

        services.Configure<EmailOptions>(emailConfigSection);
        services.AddTransient<IEmailService, EmailService>();

        return services;
    }
}