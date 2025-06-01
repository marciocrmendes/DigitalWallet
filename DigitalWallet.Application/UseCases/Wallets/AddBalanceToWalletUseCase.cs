using DigitalWallet.Application.Interfaces;
using DigitalWallet.CrossCutting.Enums;
using DigitalWallet.CrossCutting.Validation;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using DigitalWallet.Domain.ValueObjects;
using static DigitalWallet.Application.UseCases.Wallets.AddBalanceToWalletUseCase;

namespace DigitalWallet.Application.UseCases.Wallets;

public class AddBalanceToWalletUseCase(
    IWalletRepository walletRepository,
    ITransactionRepository transactionRepository
) : IUseCase<Request, Result<Response>>
{
    public record Request(
        Guid WalletId,
        decimal Amount,
        string Description,
        string? Reference = null
    );

    public record Response(
        Guid TransactionId,
        Guid WalletId,
        decimal Amount,
        string Currency,
        decimal NewBalance,
        string Description,
        DateTime CreatedAt
    );

    public async Task<Result<Response>> ExecuteAsync(Request request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var wallet = await walletRepository.GetByIdAsync(request.WalletId);
        if (wallet == null)
        {
            return Result<Response>.Failure(Wallet.ValidationError.WalletNotFound());
        }

        var amount = new Money(request.Amount, wallet.Balance.Currency);

        var transaction = new Transaction(
            walletId: wallet.Id,
            amount: amount,
            type: TransactionType.Credit,
            description: request.Description,
            reference: request.Reference
        );

        wallet.CalculateBalance(transaction);

        await transactionRepository.CreateAsync(transaction);
        await walletRepository.UpdateAsync(wallet);

        var response = new Response(
            transaction.Id,
            wallet.Id,
            amount.Amount,
            amount.Currency.ToString(),
            wallet.Balance.Amount,
            transaction.Description,
            transaction.CreatedAt
        );

        return Result<Response>.Success(response);
    }
}
