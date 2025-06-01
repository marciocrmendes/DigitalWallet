using DigitalWallet.API.Extensions;
using DigitalWallet.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services
    .AddOpenApi()
    .AddEndpointsApiExplorer()
    .AddApplicationDepencencies(configuration)
    .AddSecurity(configuration)
    .AddExceptionHandler();

var app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwaggerDependencies()
    .UseCors("AllowAll")
    .UsePathBase("/digital-wallet")
    .UseForwardedHeaders()
    .UseRouting();

app.UseSecurity();

app.RegisterEndpoints();

await app.RunAsync();
