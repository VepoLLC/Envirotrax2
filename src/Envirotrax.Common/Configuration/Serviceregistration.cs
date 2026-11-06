

using Envirotrax.Common.Data.Services.Definitions;
using Envirotrax.Common.Data.Services.Implementations;
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.Common.Domain.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

    public static IServiceCollection AddAuthService(this IServiceCollection services)
    {
        return services
            .AddHttpContextAccessor()
            .AddTransient<IAuthService, AuthService>();
    }

    public static IServiceCollection AddKeyHashingService(this IServiceCollection services)
    {
        return services
            .AddTransient<IKeyHashingService, KeyHashingService>();
    }

    public static IServiceCollection AddHtmlTemplateService(this IServiceCollection services, string? baseUrl, Action<HtmlTemplateOptions> templateConfigAction)
    {
        var validBaseUrl = GetValidBaseUrl(baseUrl);

        services.Configure<HtmlTemplateOptions>(opts =>
        {
            templateConfigAction(opts);
            opts.BaseUrl = validBaseUrl;
        });

        services.TryAddSingleton<IHtmlTemplateService, HtmlTemplateService>();

        return services;
    }

    private static string GetValidBaseUrl(string? baseUrl)
    {
        var isValid = Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);

        if (!isValid)
        {
            throw new InvalidOperationException($"A valid absolute http(s) base URL is required for templates, but '{baseUrl}' was provided.");
        }

        return uri!.AbsoluteUri.TrimEnd('/');
    }

    public static IServiceCollection AddPdfTemplateService(
        this IServiceCollection services,
        bool isDevelopment,
        string? baseUrl,
        Action<HtmlTemplateOptions> templateConfigAction,
        IConfigurationSection? pdfConfigSection = null)
    {
        services.AddHtmlTemplateService(baseUrl, templateConfigAction);

        services.Configure<PdfTemplateOptions>(opts =>
        {
            opts.IsDevelopment = isDevelopment;
            pdfConfigSection?.Bind(opts);
        });
        services.AddTransient<IPdfTemplateService, PdfTemplateService>();
        return services;
    }

    public static IServiceCollection AddEmailService(this IServiceCollection services, IConfigurationSection emailConfigSection, string? baseUrl, Action<HtmlTemplateOptions> templateConfigAction)
    {
        services.AddHtmlTemplateService(baseUrl, templateConfigAction);

        services.Configure<EmailOptions>(emailConfigSection);
        services.AddSingleton<IEmailService, EmailService>();

        return services;
    }

    public static IServiceCollection AddSmsService(this IServiceCollection services, IConfigurationSection smsConfigSection)
    {
        services.AddHttpClient();
        services.Configure<SmsOptions>(smsConfigSection);
        services.AddTransient<ISmsService, SmsService>();

        return services;
    }

    public static IServiceCollection AddRecaptchaService(this IServiceCollection services, IConfigurationSection recaptchaConfigSection)
    {
        services.AddHttpClient();
        services.Configure<RecaptchaOptions>(recaptchaConfigSection);
        services.AddTransient<IRecaptchaVerificationService, RecaptchaVerificationService>();

        return services;
    }

    public static IServiceCollection AddInternalApi<TOptions>(this IServiceCollection services, IConfigurationSection configuration)
        where TOptions : InternalApiOptions
    {
        return services
            .AddHttpClient()
            .Configure<TOptions>(configuration)
            .AddTransient<IInternalApiClientService<TOptions>, InternalApiClientService<TOptions>>();
    }

    public static IServiceCollection AddInternalApi(this IServiceCollection services, IConfigurationSection configuration)
    {
        return services
            .AddHttpClient()
            .Configure<InternalApiOptions>(configuration)
            .AddTransient<IInternalApiClientService, InternalApiClientService>();
    }

    public static IServiceCollection AddQueueService<TOptions>(this IServiceCollection services, IConfigurationSection configuration)
        where TOptions : QueueOptions
    {
        return services
            .Configure<TOptions>(configuration)
            .AddTransient<IQueueService<TOptions>, QueueService<TOptions>>();
    }

    public static IServiceCollection AddQueueService(this IServiceCollection services, IConfigurationSection configuration)
    {
        return services
            .Configure<QueueOptions>(configuration)
            .AddTransient<IQueueService, QueueService>();
    }
}