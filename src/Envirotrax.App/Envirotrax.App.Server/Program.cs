using System.Globalization;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Envirotrax.App.Server.Configuration;
using Envirotrax.App.Server.Filters;
using Envirotrax.App.Server.MediaTypeFormatters;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment())
{
    builder
        .Services
        .AddOpenTelemetry()
        .UseAzureMonitor(options =>
        {
            options.ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"] ?? throw new InvalidOperationException();
            options.Credential = new DefaultAzureCredential();
        });

}

// Add services to the container.
var keyVaultUrl = builder.Configuration["KeyVault:Url"];
if (!string.IsNullOrEmpty(keyVaultUrl))
{
    builder.Configuration.AddAzureKeyVault(
        vaultUri: new Uri(keyVaultUrl),
        credential: new DefaultAzureCredential());
}


builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(CheckFeaturesFilter));
    options.Filters.Add(typeof(CheckPermissionFilter));
    options.Filters.Add(typeof(QueryFilter));
    options.Filters.Add(typeof(ApiExceptionFilter));

    options.OutputFormatters.Add(new CsvMediaTypeFormatter());
    options.OutputFormatters.Add(new ExcelMediaTypeFormatter());
    options.OutputFormatters.Add(new XmlMediaTypeFormatter());
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(CultureInfo.InvariantCulture),
    SupportedCultures = [CultureInfo.InvariantCulture],
    SupportedUICultures = [CultureInfo.InvariantCulture]
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
