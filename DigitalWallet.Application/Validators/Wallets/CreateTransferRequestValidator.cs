using DigitalWallet.Application.UseCases.Wallets;
using FluentValidation;

namespace DigitalWallet.Application.Validators.Wallets;

public class CreateTransferRequestValidator : AbstractValidator<CreateTransferUseCase.Request>
{
    public CreateTransferRequestValidator()
    {
        RuleFor(x => x.FromWalletId)
            .NotEmpty()
            .WithMessage("ID da carteira de origem é obrigatório")
            .NotEqual(Guid.Empty)
            .WithMessage("ID da carteira de origem deve ser um GUID válido");

        RuleFor(x => x.ToWalletId)
            .NotEmpty()
            .WithMessage("ID da carteira de destino é obrigatório")
            .NotEqual(Guid.Empty)
            .WithMessage("ID da carteira de destino deve ser um GUID válido");

        RuleFor(x => x)
            .Must(x => x.FromWalletId != x.ToWalletId)
            .WithMessage("Carteira de origem deve ser diferente da carteira de destino");
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Valor deve ser maior que zero")
            .Must(HasMaxTwoDecimalPlaces)
            .WithMessage("Valor deve ter no máximo 2 casas decimais");

        RuleFor(x => x.Currency)
            .IsInEnum()
            .WithMessage("Moeda deve ser válida (BRL, USD, EUR, GBP, JPY)");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Descrição é obrigatória")
            .MinimumLength(5)
            .WithMessage("Descrição deve ter pelo menos 5 caracteres")
            .MaximumLength(255)
            .WithMessage("Descrição deve ter no máximo 255 caracteres");
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
