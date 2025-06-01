using DigitalWallet.Application.UseCases.Users;
using FluentValidation;

namespace DigitalWallet.Application.Validators.Users;

public class CreateUserRequestValidator : AbstractValidator<CreateUserUseCase.Request>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Nome é obrigatório")
            .MinimumLength(2)
            .WithMessage("Nome deve ter pelo menos 2 caracteres")
            .MaximumLength(50)
            .WithMessage("Nome deve ter no máximo 50 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("Nome deve conter apenas letras e espaços");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Sobrenome é obrigatório")
            .MinimumLength(2)
            .WithMessage("Sobrenome deve ter pelo menos 2 caracteres")
            .MaximumLength(50)
            .WithMessage("Sobrenome deve ter no máximo 50 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("Sobrenome deve conter apenas letras e espaços");

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
            .WithMessage("Senha deve ter pelo menos 8 caracteres")
            .MaximumLength(32)
            .WithMessage("Senha deve ter no máximo 32 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage(
                "Senha deve conter pelo menos uma letra minúscula, uma maiúscula, um número e um caractere especial"
            );
    }
}
