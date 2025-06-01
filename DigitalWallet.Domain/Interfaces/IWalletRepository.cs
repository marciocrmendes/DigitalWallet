using DigitalWallet.Domain.Entities;

namespace DigitalWallet.Domain.Interfaces;

public interface IWalletRepository
{
    Task<Wallet?> GetByIdAsync(Guid id);
    Task<IReadOnlyCollection<Wallet>> GetByUserIdAsync(Guid userId);
    Task<Wallet> CreateAsync(Wallet wallet);
    Task CreateRangeAsync(params IEnumerable<Wallet> wallets);
    Task<Wallet> UpdateAsync(Wallet wallet);
    Task UpdateRangeAsync(params IEnumerable<Wallet> wallets);
    Task DeleteAsync(Wallet wallet);
}
