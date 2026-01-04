using Envirotrax.Auth.Data;
using Envirotrax.Auth.Data.Configuration;
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Domain.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapControllers();

app.Run();
