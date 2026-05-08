using Azure.Identity;
using Envirotrax.TaskRunner.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Configuration.AddAzureKeyVault(
    vaultUri: new Uri(builder.Configuration["KeyVault:Url"] ?? throw new InvalidOperationException()),
    credential: new DefaultAzureCredential());

builder.Services.AddTaskRunnerServices(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

var controllerBuilder = app.MapControllers();

if (!builder.Environment.IsDevelopment())
{
    controllerBuilder.RequireAuthorization();
}

app.Run();
