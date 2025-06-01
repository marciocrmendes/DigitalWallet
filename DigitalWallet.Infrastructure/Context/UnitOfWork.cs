using DigitalWallet.Domain.Interfaces.UnitOfWork;

namespace DigitalWallet.Infrastructure.Context
{
    public class UnitOfWork(DigitalWalletDbContext context) : IUnitOfWork
    {
        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
    }
}
