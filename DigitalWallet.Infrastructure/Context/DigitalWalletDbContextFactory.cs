using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DigitalWallet.Infrastructure.Context;

public class DigitalWalletDbContextFactory : IDesignTimeDbContextFactory<DigitalWalletDbContext>
{
    public DigitalWalletDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DigitalWalletDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Database=digitalwallet_dev;Username=postgres;Password=postgres"
        );

        return new DigitalWalletDbContext(optionsBuilder.Options);
    }
}
