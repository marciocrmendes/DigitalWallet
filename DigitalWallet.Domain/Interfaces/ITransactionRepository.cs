using DigitalWallet.Domain.Entities;

namespace DigitalWallet.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id);
    Task<IEnumerable<Transaction>> GetByWalletIdAsync(Guid walletId);
    Task<IEnumerable<Transaction>> GetByUserIdAsync(
        Guid userId,
        DateTime? startDate = null,
        DateTime? endDate = null
    );
    Task<Transaction> CreateAsync(Transaction transaction);
    Task CreateRangeAsync(params IEnumerable<Transaction> transactions);
    Task<Transaction> UpdateAsync(Transaction transaction);
    Task DeleteAsync(Transaction transaction);
}
