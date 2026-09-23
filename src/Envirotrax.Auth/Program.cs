using System.Net;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Envirotrax.Auth.Data;
using Envirotrax.Auth.Data.Configuration;
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Domain.Configuration;
using Envirotrax.Auth.Domain.Security;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.EntityFrameworkCore;


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
builder.Configuration.AddAzureKeyVault(
    vaultUri: new Uri(builder.Configuration["KeyVault:Url"] ?? throw new InvalidOperationException()),
    credential: new DefaultAzureCredential());

builder
    .Services
    .AddDataServices(builder.Configuration, builder.Environment)
    .AddDomainServices(builder.Configuration, builder.Environment);

var razor = builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    razor.AddRazorRuntimeCompilation();
}

var allowedCorsOrigins = "_alowedCorsOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedCorsOrigins, policy =>
    {
        var origins = builder.Configuration["Cors:AllowedOrigins"] ?? throw new InvalidOperationException("No CORS configuration was provided");

        policy.WithOrigins(origins.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors(allowedCorsOrigins);

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// A user with an expired password (e.g. a legacy-migrated account) is fully authenticated by
// the cookie but must not be able to reach anything else - including the OpenIddict /connect/authorize
// endpoint the client apps use to mint tokens - until they set a new password. Enforced here, on every
// request, rather than only at the login form, so there is no page or deep link that bypasses it.
app.Use(async (context, next) =>
{
    var endpoint = context.GetEndpoint();
    var isPageOrControllerEndpoint = endpoint?.Metadata.GetMetadata<ActionDescriptor>() != null;
    var isExemptFromExpiredPasswordCheck = endpoint?.Metadata.GetMetadata<AllowExpiredPasswordAttribute>() != null;

    if (isPageOrControllerEndpoint
        && !isExemptFromExpiredPasswordCheck
        && context.User.HasClaim(AppClaimTypes.PasswordExpired, "true"))
    {
        var returnUrl = context.Request.Path + context.Request.QueryString;
        context.Response.Redirect($"/Identity/Account/ChangeExpiredPassword?ReturnUrl={Uri.EscapeDataString(returnUrl)}");
        return;
    }

    await next();
});

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapControllers();

app.Run();
