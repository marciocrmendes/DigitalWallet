using DigitalWallet.Application.DTOs.Transaction;
using DigitalWallet.Domain.Entities;

namespace DigitalWallet.Application.Extensions
{
    public static class TransactionExtensions
    {
        public static IReadOnlyCollection<TransactionDto> ToDtoList(
            this IEnumerable<Transaction> transactions
        )
        {
            var transactionArray = transactions.ToArray();

            if (transactionArray?.Length == 0)
                return [];
            return
            [
                .. transactionArray!.Select(transaction => new TransactionDto
                {
                    Id = transaction.Id,
                    WalletId = transaction.WalletId,
                    Amount = transaction.Amount.Amount,
                    Currency = transaction.Amount.Currency.ToString(),
                    Type = transaction.Type.ToString(),
                    Description = transaction.Description,
                    Reference = transaction.Reference,
                    Status = transaction.Status.ToString(),
                    CreatedAt = transaction.CreatedAt,
                    ProcessedAt = transaction.ProcessedAt,
                }),
            ];
        }
    }
}
