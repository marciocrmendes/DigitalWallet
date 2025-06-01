using DigitalWallet.Application.Interfaces;
using DigitalWallet.CrossCutting.Validation;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using static DigitalWallet.Application.UseCases.Users.GetUserByIdUseCase;

namespace DigitalWallet.Application.UseCases.Users;

public class GetUserByIdUseCase(IUserRepository userRepository)
    : IUseCase<Request, Result<Response>>
{
    public record Request(Guid UserId);

    public record Response(Guid UserId, string UserFullName, string UserEmail);

    public async Task<Result<Response>> ExecuteAsync(Request request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var user = await userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return Result<Response>.Failure(User.ValidationError.UserNotFound());
        }

        var response = new Response(user.Id, user.FullName, user.Email!);

        return Result<Response>.Success(response);
    }
}
