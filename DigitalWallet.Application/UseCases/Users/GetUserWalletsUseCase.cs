using DigitalWallet.Application.DTOs.Wallet;
using DigitalWallet.Application.Extensions;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.CrossCutting.Validation;
using DigitalWallet.Domain.Interfaces;
using static DigitalWallet.Application.UseCases.Users.GetUserWalletsUseCase;

namespace DigitalWallet.Application.UseCases.Users;

public class GetUserWalletsUseCase(IWalletRepository walletRepository)
    : IUseCase<Request, Result<Response>>
{
    public record Request(Guid UserId);

    public record Response(IReadOnlyCollection<WalletDto>? Wallets);

    public async Task<Result<Response>> ExecuteAsync(Request request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var wallets = await walletRepository.GetByUserIdAsync(request.UserId);
        if (wallets?.Count == 0)
        {
            return Result<Response>.Success(new Response(default));
        }

        return Result<Response>.Success(new Response(wallets!.ToDtoList()));
    }
}
