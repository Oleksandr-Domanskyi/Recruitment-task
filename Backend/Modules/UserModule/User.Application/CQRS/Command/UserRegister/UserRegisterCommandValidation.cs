
using FluentValidation;

namespace User.Application.CQRS.Command.UserRegister;

public sealed class UserRegisterCommandValidation
    : AbstractValidator<UserRegisterCommand>
{
    public UserRegisterCommandValidation()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MinimumLength(3)
            .WithMessage("Username must contain at least 3 characters.")
            .MaximumLength(50)
            .WithMessage("Username must not exceed 50 characters.")
            .Matches("^[a-zA-Z0-9]+$")
            .WithMessage("Username can contain only Latin letters and numbers.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(256)
            .WithMessage("Email must not exceed 256 characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(12)
            .WithMessage("Password must contain at least 12 characters.")
            .MaximumLength(128)
            .WithMessage("Password must not exceed 128 characters.");
    }
}
