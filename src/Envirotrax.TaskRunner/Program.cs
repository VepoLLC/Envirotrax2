using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Envirotrax.TaskRunner.Configuration;

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
builder.Services.AddControllers();

builder.Configuration.AddAzureKeyVault(
    vaultUri: new Uri(builder.Configuration["KeyVault:Url"] ?? throw new InvalidOperationException()),
    credential: new DefaultAzureCredential());

builder.Services.AddTaskRunnerServices(builder.Configuration);

var app = builder.Build();

// This doesn't work for internally bound sites in IIS.
//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

var controllerBuilder = app.MapControllers();

if (!builder.Environment.IsDevelopment())
{
    controllerBuilder.RequireAuthorization();
}

app.Run();
