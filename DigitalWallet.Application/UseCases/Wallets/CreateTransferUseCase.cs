using DigitalWallet.Application.DTOs.Transaction;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.CrossCutting.Enums;
using DigitalWallet.CrossCutting.Validation;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using DigitalWallet.Domain.ValueObjects;
using static DigitalWallet.Application.UseCases.Wallets.CreateTransferUseCase;

namespace DigitalWallet.Application.UseCases.Wallets;

public class CreateTransferUseCase(
    IWalletRepository walletRepository,
    ITransactionRepository transactionRepository
) : IUseCase<Request, Result<Response>>
{
    public record Request(
        Guid FromWalletId,
        Guid ToWalletId,
        decimal Amount,
        Currency Currency,
        string Description,
        string? Reference = null
    );

    public record Response(
        Guid FromTransactionId,
        Guid ToTransactionId,
        Guid FromWalletId,
        Guid ToWalletId,
        decimal Amount,
        string Currency,
        string Description,
        DateTime CreatedAt
    );

    public async Task<Result<Response>> ExecuteAsync(Request request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var validationResult = await ValidateTransferAsync(request);
        if (validationResult?.Result?.IsSuccess == false)
            return validationResult.Result;

        var fromWallet = validationResult!.FromWallet;
        var toWallet = validationResult!.ToWallet;
        var transferId = Guid.NewGuid();

        var (debitTransaction, creditTransaction) = BuildTransferTransactions(
            request,
            fromWallet,
            toWallet,
            transferId
        );

        await ProcessTransferTransactionsAsync(
            fromWallet,
            toWallet,
            debitTransaction,
            creditTransaction
        );

        var response = new Response(
            debitTransaction.Id,
            creditTransaction.Id,
            fromWallet.Id,
            toWallet.Id,
            debitTransaction.Amount.Amount,
            debitTransaction.Amount.Currency.ToString(),
            request.Description,
            debitTransaction.CreatedAt
        );

        return Result<Response>.Success(response);
    }

    private async Task ProcessTransferTransactionsAsync(
        Wallet fromWallet,
        Wallet toWallet,
        Transaction debitTransaction,
        Transaction creditTransaction
    )
    {
        fromWallet.CalculateBalance(debitTransaction);
        toWallet.CalculateBalance(creditTransaction);

        await transactionRepository.CreateRangeAsync(creditTransaction, debitTransaction);

        await walletRepository.UpdateRangeAsync(fromWallet, toWallet);
    }

    private static (Transaction debit, Transaction credit) BuildTransferTransactions(
        Request request,
        Wallet fromWallet,
        Wallet toWallet,
        Guid transferId
    )
    {
        var debitTransaction = new Transaction(
            walletId: fromWallet.Id,
            amount: new Money(request.Amount, request.Currency),
            type: TransactionType.Debit,
            description: $"Transfer to wallet {toWallet.Id}: {request.Description}",
            reference: string.IsNullOrEmpty(request.Reference)
                ? $"TRANSFER-{transferId}"
                : request.Reference
        );

        var creditTransaction = new Transaction(
            walletId: toWallet.Id,
            amount: new Money(request.Amount, request.Currency),
            type: TransactionType.Credit,
            description: $"Transfer from wallet {fromWallet.Id}: {request.Description}",
            reference: string.IsNullOrEmpty(request.Reference)
                ? $"TRANSFER-{transferId}"
                : request.Reference
        );

        return (debitTransaction, creditTransaction);
    }

    private async Task<TransferValidationAggregate> ValidateTransferAsync(Request request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var response = new TransferValidationAggregate();

        if (request.Amount <= 0)
        {
            response.Result = Result<Response>.Failure(Transaction.ValidationError.InvalidAmount());
            return response;
        }

        if (request.FromWalletId == request.ToWalletId)
        {
            response.Result = Result<Response>.Failure(Transaction.ValidationError.SameWallet());
            return response;
        }

        var fromWallet = await walletRepository.GetByIdAsync(request.FromWalletId);
        if (fromWallet == null)
        {
            response.Result = Result<Response>.Failure(
                Transaction.ValidationError.FromWalletNotFound()
            );
            return response;
        }

        var toWallet = await walletRepository.GetByIdAsync(request.ToWalletId);
        if (toWallet == null)
        {
            response.Result = Result<Response>.Failure(
                Transaction.ValidationError.ToWalletNotFound()
            );
            return response;
        }

        if (fromWallet.Balance.Currency != toWallet.Balance.Currency)
        {
            response.Result = Result<Response>.Failure(Wallet.ValidationError.InvalidCurrency());
            return response;
        }

        var transferAmount = new Money(request.Amount, fromWallet.Balance.Currency);

        if (!fromWallet.CanDebit(transferAmount))
        {
            response.Result = Result<Response>.Failure(Wallet.ValidationError.InsufficientFunds());
            return response;
        }

        response.FromWallet = fromWallet;
        response.ToWallet = toWallet;

        return response;
    }
}
