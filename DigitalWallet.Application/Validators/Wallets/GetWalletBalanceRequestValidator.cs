using DigitalWallet.Application.UseCases.Wallets;
using FluentValidation;

namespace DigitalWallet.Application.Validators.Wallets;

public class GetWalletBalanceRequestValidator : AbstractValidator<GetWalletBalanceUseCase.Request>
{
    public GetWalletBalanceRequestValidator()
    {
        RuleFor(x => x.WalletId)
            .NotEmpty()
            .WithMessage("ID da carteira é obrigatório")
            .NotEqual(Guid.Empty)
            .WithMessage("ID da carteira deve ser um GUID válido");
    }
}
