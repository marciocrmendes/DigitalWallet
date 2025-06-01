using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Infrastructure.Seeders;

public class UserSeeder(UserManager<User> userManager, ILogger<UserSeeder> logger)
{
    public async Task SeedAsync()
    {
        logger.LogInformation("Starting user seeding process...");

        await SeedUserAsync(
            firstName: "Carlos",
            lastName: "Silva",
            email: "carlos.silva@digitalwallet.com",
            password: "Password123!"
        );

        await SeedUserAsync(
            firstName: "Maria",
            lastName: "Santos",
            email: "maria.santos@digitalwallet.com",
            password: "Password123!"
        );

        logger.LogInformation("User seeding process completed.");
    }

    private async Task SeedUserAsync(
        string firstName,
        string lastName,
        string email,
        string password
    )
    {
        try
        {
            // Check if user already exists
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                logger.LogInformation("User with email {Email} already exists, skipping...", email);
                return;
            }

            // Create email value object
            var emailValueObject = new Email(email);

            // Create user entity
            var user = new User(firstName, lastName, emailValueObject);

            // Create user with UserManager (this handles password hashing)
            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                logger.LogInformation(
                    "Successfully created user: {FullName} ({Email}) with ID: {UserId}",
                    user.FullName,
                    user.Email,
                    user.Id
                );

                // Log wallet information
                if (user.Wallets.Any())
                {
                    var defaultWallet = user.Wallets.First();
                    logger.LogInformation(
                        "Default wallet created for user {Email}: {WalletName} with balance {Balance}",
                        user.Email,
                        defaultWallet.Name,
                        defaultWallet.Balance.Amount
                    );
                }
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogError("Failed to create user {Email}. Errors: {Errors}", email, errors);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while seeding user {Email}: {Message}",
                email,
                ex.Message
            );
        }
    }
}
