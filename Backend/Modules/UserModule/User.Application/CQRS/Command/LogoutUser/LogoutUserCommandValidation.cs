
using FluentValidation;

namespace User.Application.CQRS.Command.LogoutUser;

public sealed class LogoutUserCommandValidation : AbstractValidator<LogoutUserCommand>
{
    public LogoutUserCommandValidation()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required.");

        RuleFor(x => x.userId)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}
