using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using DigitalWallet.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Infrastructure.Repositories;

public sealed class WalletRepository(DigitalWalletDbContext context)
    : BaseRepository<Wallet>(context),
        IWalletRepository
{
    private readonly DigitalWalletDbContext _context = context;

    public async Task<Wallet?> GetByIdAsync(Guid id)
    {
        return await Repository.Include(w => w.Transactions).FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<IReadOnlyCollection<Wallet>> GetByUserIdAsync(Guid userId)
    {
        return await Repository
            .Include(w => w.Transactions)
            .Where(w => w.UserId == userId)
            .ToListAsync();
    }

    public async Task<Wallet> CreateAsync(Wallet wallet)
    {
        Repository.Add(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }

    public async Task CreateRangeAsync(params IEnumerable<Wallet> wallets)
    {
        await Repository.AddRangeAsync(wallets);
        await _context.SaveChangesAsync();
    }

    public async Task<Wallet> UpdateAsync(Wallet wallet)
    {
        Repository.Update(wallet);
        await _context.SaveChangesAsync();

        return wallet;
    }

    public async Task UpdateRangeAsync(params IEnumerable<Wallet> wallets)
    {
        Repository.UpdateRange(wallets);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Wallet wallet)
    {
        Repository.Remove(wallet);
        await _context.SaveChangesAsync();
    }
}
