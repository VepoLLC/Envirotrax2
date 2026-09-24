using System.Diagnostics;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Envirotrax.Admin.Server.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilter>();
});

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? context.HttpContext.TraceIdentifier;

        context.ProblemDetails.Extensions["traceId"] = traceId;
    };
});

builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
