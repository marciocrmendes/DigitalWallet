using DigitalWallet.Application.Interfaces;
using DigitalWallet.CrossCutting.Enums;
using DigitalWallet.CrossCutting.Validation;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using static DigitalWallet.Application.UseCases.Wallets.CreateWalletUseCase;

namespace DigitalWallet.Application.UseCases.Wallets;

public class CreateWalletUseCase(IWalletRepository walletRepository, IUserRepository userRepository)
    : IUseCase<Request, Result<Response>>
{
    public record Request(Guid UserId, string Name, string? Description, Currency Currency);

    public record Response(
        Guid Id,
        Guid UserId,
        string Name,
        string? Description,
        decimal Balance,
        string Currency,
        string Status,
        DateTime CreatedAt
    );

    public async Task<Result<Response>> ExecuteAsync(Request request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var user = await userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return Result<Response>.Failure(Wallet.ValidationError.UserNotFound());
        }

        var wallet = new Wallet(
            request.UserId,
            request.Name,
            request.Currency,
            request.Description
        );

        await walletRepository.CreateAsync(wallet);

        return Result<Response>.Success(
            new Response(
                wallet.Id,
                wallet.UserId,
                wallet.Name,
                wallet.Description,
                wallet.Balance.Amount,
                wallet.Balance.Currency.ToString(),
                wallet.Status.ToString(),
                wallet.CreatedAt
            )
        );
    }
}
