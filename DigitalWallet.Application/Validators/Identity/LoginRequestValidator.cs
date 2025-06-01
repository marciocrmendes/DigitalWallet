using DigitalWallet.Application.UseCases.Identity;
using FluentValidation;

namespace DigitalWallet.Application.Validators.Identity;

public class LoginRequestValidator : AbstractValidator<LoginUseCase.Request>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email é obrigatório")
            .EmailAddress()
            .WithMessage("Email deve ter um formato válido")
            .MaximumLength(100)
            .WithMessage("Email deve ter no máximo 100 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Senha é obrigatória")
            .MinimumLength(8)
            .WithMessage("Senha deve ter pelo menos 6 caracteres")
            .MaximumLength(32)
            .WithMessage("Senha deve ter no máximo 32 caracteres");
    }
}
