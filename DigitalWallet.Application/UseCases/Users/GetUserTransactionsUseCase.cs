using DigitalWallet.Application.DTOs.Transaction;
using DigitalWallet.Application.Extensions;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.CrossCutting.Validation;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using static DigitalWallet.Application.UseCases.Users.GetUserTransactionsUseCase;

namespace DigitalWallet.Application.UseCases.Users;

public class GetUserTransactionsUseCase(
    ITransactionRepository transactionRepository,
    IUserRepository userRepository
) : IUseCase<Request, Result<Response>>
{
    public record Request(Guid UserId, DateTime? StartDate = null, DateTime? EndDate = null);

    public record Response(IReadOnlyCollection<TransactionDto> Transactions);

    public async Task<Result<Response>> ExecuteAsync(Request request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var user = await userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return Result<Response>.Failure(User.ValidationError.UserNotFound());
        }

        var transactions = await transactionRepository.GetByUserIdAsync(
            request.UserId,
            request.StartDate,
            request.EndDate
        );

        var response = new Response(transactions.ToDtoList());

        return Result<Response>.Success(response);
    }
}
