using DigitalWallet.Domain.Entities;
using DigitalWallet.Infrastructure.Context;
using DigitalWallet.Infrastructure.Seeders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Infrastructure.Extensions;

public static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DigitalWalletDbContext>>();
        var context = scope.ServiceProvider.GetRequiredService<DigitalWalletDbContext>();

        try
        {
            // Apply migrations
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");

            // Seed data
            await SeedDataAsync(scope.ServiceProvider, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while initializing the database: {message}",
                ex.Message
            );
            throw;
        }
    }

    private static async Task SeedDataAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        try
        {
            logger.LogInformation("Starting database seeding process...");

            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var seederLogger = serviceProvider.GetRequiredService<ILogger<UserSeeder>>();

            var userSeeder = new UserSeeder(userManager, seederLogger);
            await userSeeder.SeedAsync();

            logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database seeding: {message}", ex.Message);
            throw;
        }
    }
}
