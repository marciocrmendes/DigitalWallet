using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using DigitalWallet.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Infrastructure.Repositories;

public sealed class TransactionRepository(DigitalWalletDbContext context)
    : BaseRepository<Transaction>(context),
        ITransactionRepository
{
    private readonly DigitalWalletDbContext _context = context;

    public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        return await Repository.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Transaction>> GetByWalletIdAsync(Guid walletId)
    {
        return await Repository
            .Where(t => t.WalletId == walletId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByUserIdAsync(
        Guid userId,
        DateTime? startDate = null,
        DateTime? endDate = null
    )
    {
        var query = Repository.Where(t =>
            _context.Wallets.Any(w => w.Id == t.WalletId && w.UserId == userId)
        );

        if (startDate.HasValue)
        {
            query = query.Where(t => t.ProcessedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(t => t.ProcessedAt <= endDate.Value);
        }

        return await query.OrderByDescending(t => t.ProcessedAt).ToListAsync();
    }

    public async Task<Transaction> CreateAsync(Transaction transaction)
    {
        Repository.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task CreateRangeAsync(params IEnumerable<Transaction> transactions)
    {
        await Repository.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();
    }

    public async Task<Transaction> UpdateAsync(Transaction transaction)
    {
        Repository.Update(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task DeleteAsync(Transaction transaction)
    {
        Repository.Remove(transaction);
        await _context.SaveChangesAsync();
    }
}
