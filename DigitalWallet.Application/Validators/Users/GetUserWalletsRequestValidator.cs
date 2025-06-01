using DigitalWallet.Application.UseCases.Users;
using FluentValidation;

namespace DigitalWallet.Application.Validators.Users;

public class GetUserWalletsRequestValidator : AbstractValidator<GetUserWalletsUseCase.Request>
{
    public GetUserWalletsRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID do usuário é obrigatório")
            .NotEqual(Guid.Empty)
            .WithMessage("ID do usuário deve ser um GUID válido");
    }
}
