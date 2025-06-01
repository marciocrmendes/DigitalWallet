using DigitalWallet.CrossCutting.Auditory;
using DigitalWallet.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Infrastructure.Repositories
{
    public abstract class BaseRepository<TEntity>(DigitalWalletDbContext context)
        where TEntity : class, IIdentifiableEntity
    {
        protected readonly DbSet<TEntity> Repository = context.Set<TEntity>();
    }
}
