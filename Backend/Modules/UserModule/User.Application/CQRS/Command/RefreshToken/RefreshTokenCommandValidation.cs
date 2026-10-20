
using FluentValidation;

namespace User.Application.CQRS.Command.RefreshToken;

public sealed class RefreshTokenCommandValidation : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidation()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required.");
    }
}
