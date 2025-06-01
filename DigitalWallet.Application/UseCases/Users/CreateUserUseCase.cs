using DigitalWallet.Application.DTOs.Wallet;
using DigitalWallet.Application.Extensions;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.CrossCutting.Validation;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using DigitalWallet.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using static DigitalWallet.Application.UseCases.Users.CreateUserUseCase;

namespace DigitalWallet.Application.UseCases.Users;

public sealed class CreateUserUseCase(UserManager<User> userManager, IUserRepository userRepository)
    : IUseCase<Request, Result<Response>>
{
    public record Request(string FirstName, string LastName, string Email, string Password);

    public record Response(
        Guid Id,
        string FullName,
        string Email,
        IReadOnlyCollection<WalletDto> Wallets
    );

    public async Task<Result<Response>> ExecuteAsync(Request request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        if (await userRepository.EmailExistsAsync(request.Email))
            return Result<Response>.Failure(User.ValidationError.EmailAlreadyExists());

        var email = new Email(request.Email);

        var user = new User(request.FirstName, request.LastName, email);

        var identityResult = await userManager.CreateAsync(user, request.Password);
        if (!identityResult.Succeeded)
        {
            return Result<Response>.Failure(
                User.ValidationError.CreateUserFailed(
                    [.. identityResult.Errors.Select(e => e.Description)]
                )
            );
        }

        var response = new Response(user.Id, user.FullName, user.Email!, user.Wallets.ToDtoList());

        return Result<Response>.Success(response);
    }
}
