using System.Net;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Envirotrax.Auth.Data;
using Envirotrax.Auth.Data.Configuration;
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Domain.Configuration;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
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

if (!builder.Environment.IsDevelopment())
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

        var ipAddress = builder.Configuration["LoadBalacerIpAddress"] ?? throw new InvalidOperationException("Load balancer IP was not found.");

        options.KnownProxies.Add(IPAddress.Parse(ipAddress));
    });
}

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

app.UseForwardedHeaders();
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapControllers();

app.Run();
