using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using DigitalWallet.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Infrastructure.Repositories;

public sealed class UserRepository(DigitalWalletDbContext context)
    : BaseRepository<User>(context),
        IUserRepository
{
    private readonly DigitalWalletDbContext _context = context;

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await Repository.Include(u => u.Wallets).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IReadOnlyCollection<User>> GetAllAsync()
    {
        return await Repository.Include(u => u.Wallets).ToListAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        Repository.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        Repository.Update(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task DeleteAsync(User user)
    {
        Repository.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await Repository.AnyAsync(u => u.Email!.ToLower() == email.ToLower());
    }
}
