using DigitalWallet.Application.UseCases.Wallets;
using FluentValidation;

namespace DigitalWallet.Application.Validators.Wallets;

public class AddBalanceToWalletRequestValidator
    : AbstractValidator<AddBalanceToWalletUseCase.Request>
{
    public AddBalanceToWalletRequestValidator()
    {
        RuleFor(x => x.WalletId)
            .NotEmpty()
            .WithMessage("ID da carteira é obrigatório")
            .NotEqual(Guid.Empty)
            .WithMessage("ID da carteira deve ser um GUID válido");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Valor deve ser maior que zero")
            .Must(HasMaxTwoDecimalPlaces)
            .WithMessage("Valor deve ter no máximo 2 casas decimais");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Descrição é obrigatória")
            .MinimumLength(5)
            .WithMessage("Descrição deve ter pelo menos 5 caracteres")
            .MaximumLength(1000)
            .WithMessage("Descrição deve ter no máximo 1000 caracteres");

        RuleFor(x => x.Reference)
            .MaximumLength(100)
            .WithMessage("Referência deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Reference));
    }

    private static bool HasMaxTwoDecimalPlaces(decimal value)
    {
        return decimal.Round(value, 2) == value;
    }
}
