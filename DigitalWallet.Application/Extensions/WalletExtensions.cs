using DigitalWallet.Application.DTOs.Wallet;
using DigitalWallet.Domain.Entities;

namespace DigitalWallet.Application.Extensions
{
    public static class WalletExtensions
    {
        public static IReadOnlyCollection<WalletDto> ToDtoList(this IEnumerable<Wallet> wallets)
        {
            var walletArray = wallets.ToArray();

            if (walletArray?.Length == 0)
                return [];

            return
            [
                .. walletArray!.Select(wallet => new WalletDto
                {
                    Id = wallet.Id,
                    Name = wallet.Name,
                    Description = wallet.Description,
                    Balance = wallet.Balance.Amount,
                    Currency = wallet.Balance.Currency.ToString(),
                }),
            ];
        }
    }
}
