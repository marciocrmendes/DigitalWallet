using DigitalWallet.Application.UseCases.Wallets;
using DigitalWallet.CrossCutting.Enums;
using FluentValidation;

namespace DigitalWallet.Application.Validators.Wallets;

public class CreateWalletRequestValidator : AbstractValidator<CreateWalletUseCase.Request>
{
    public CreateWalletRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID do usuário é obrigatório")
            .NotEqual(Guid.Empty)
            .WithMessage("ID do usuário deve ser um GUID válido");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome da carteira é obrigatório")
            .MinimumLength(3)
            .WithMessage("Nome da carteira deve ter pelo menos 3 caracteres")
            .MaximumLength(200)
            .WithMessage("Nome da carteira deve ter no máximo 200 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ0-9\s\-_#()]+$")
            .WithMessage("Nome da carteira contém caracteres inválidos");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Descrição deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Currency)
            .IsInEnum()
            .WithMessage("Moeda deve ser válida (BRL, USD, EUR, GBP, JPY)");
    }
}
