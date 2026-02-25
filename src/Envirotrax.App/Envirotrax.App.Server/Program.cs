using Azure.Identity;
using Envirotrax.App.Server.Configuration;
using Envirotrax.App.Server.Filters;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddAzureKeyVault(
    vaultUri: new Uri(builder.Configuration["KeyVault:Url"] ?? throw new InvalidOperationException()),
    credential: new DefaultAzureCredential());

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(CheckFeaturesFilter));
});
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
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
