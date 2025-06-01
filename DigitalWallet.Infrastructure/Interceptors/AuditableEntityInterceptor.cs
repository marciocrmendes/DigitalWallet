using DigitalWallet.CrossCutting;
using DigitalWallet.CrossCutting.Auditory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DigitalWallet.Infrastructure.Interceptors
{
    public class AuditableEntityInterceptor(AppUser appUser) : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result
        )
        {
            UpdateEntities(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default
        )
        {
            UpdateEntities(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public void UpdateEntities(DbContext? context)
        {
            if (context == null)
                return;

            var dateTime = DateTime.UtcNow;

            foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
            {
                if (entry.State is EntityState.Added or EntityState.Modified)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.CreatedBy = appUser.UserId;
                        entry.Entity.CreatedAt = dateTime;
                    }

                    entry.Entity.UpdatedBy = appUser.UserId;
                    entry.Entity.UpdatedAt = dateTime;
                }
            }
        }
    }
}
