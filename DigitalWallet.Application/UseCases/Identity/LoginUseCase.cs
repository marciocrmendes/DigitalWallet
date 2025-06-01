using DigitalWallet.Application.Interfaces;
using DigitalWallet.Application.Providers;
using DigitalWallet.CrossCutting.Options;
using DigitalWallet.CrossCutting.Validation;
using DigitalWallet.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using static DigitalWallet.Application.UseCases.Identity.LoginUseCase;

namespace DigitalWallet.Application.UseCases.Identity;

public sealed class LoginUseCase(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IOptions<JwtSettings> jwtSettings,
    TokenProvider tokenProvider
) : IUseCase<Request, Result<Response>>
{
    public record Request(string Email, string Password);

    public record Response(
        Guid UserId,
        string UserFullName,
        string UserEmail,
        string Token,
        DateTime ExpiresAt
    );

    public async Task<Result<Response>> ExecuteAsync(Request request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result<Response>.Failure(User.ValidationError.UserNotFound());
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            false
        );
        if (!signInResult.Succeeded)
        {
            return Result<Response>.Failure(User.ValidationError.UserInvalidLoginOrPassword());
        }

        var token = await tokenProvider.GenerateJwtTokenAsync(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(jwtSettings.Value.TokenExpirationInMinutes);

        var response = new Response(
            user.Id,
            user.FullName,
            user.Email!,
            $"Bearer {token}",
            expiresAt
        );

        return Result<Response>.Success(response);
    }
}
