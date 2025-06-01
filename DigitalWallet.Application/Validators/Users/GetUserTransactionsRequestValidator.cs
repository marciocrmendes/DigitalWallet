using DigitalWallet.Application.UseCases.Users;
using FluentValidation;

namespace DigitalWallet.Application.Validators.Users;

public class GetUserTransactionsRequestValidator
    : AbstractValidator<GetUserTransactionsUseCase.Request>
{
    public GetUserTransactionsRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID do usuário é obrigatório")
            .NotEqual(Guid.Empty)
            .WithMessage("ID do usuário deve ser um GUID válido");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("Data de início deve ser anterior ou igual à data de fim");

        RuleFor(x => x.EndDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.EndDate.HasValue)
            .WithMessage("Data de fim não pode ser no futuro");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.StartDate.HasValue)
            .WithMessage("Data de início não pode ser no futuro");
    }
}
