using DigitalWallet.Application.Interfaces;
using FluentValidation;

namespace DigitalWallet.Application.Decorators;

public class ValidationUseCaseDecorator<TRequest, TResponse>(
    IUseCase<TRequest, TResponse> useCase,
    IValidator<TRequest>? validator = null
) : IUseCase<TRequest, TResponse>
    where TRequest : class
{
    public async Task<TResponse> ExecuteAsync(TRequest request)
    {
        if (validator != null)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }

        return await useCase.ExecuteAsync(request);
    }
}
