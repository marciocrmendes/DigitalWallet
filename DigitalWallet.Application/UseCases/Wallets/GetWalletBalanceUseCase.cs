using DigitalWallet.Application.Interfaces;
using DigitalWallet.CrossCutting.Validation;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using static DigitalWallet.Application.UseCases.Wallets.GetWalletBalanceUseCase;

namespace DigitalWallet.Application.UseCases.Wallets;

public class GetWalletBalanceUseCase(IWalletRepository walletRepository)
    : IUseCase<Request, Result<Response>>
{
    public record Request(Guid WalletId);

    public record Response(Guid WalletId, decimal Balance, string Currency, string Status);

    public async Task<Result<Response>> ExecuteAsync(Request request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var wallet = await walletRepository.GetByIdAsync(request.WalletId);
        if (wallet == null)
        {
            return Result<Response>.Failure(Wallet.ValidationError.WalletNotFound());
        }

        var response = new Response(
            wallet.Id,
            wallet.Balance.Amount,
            wallet.Balance.Currency.ToString(),
            wallet.Status.ToString()
        );

        return Result<Response>.Success(response);
    }
}
