
using FluentValidation;

namespace User.Application.CQRS.Command.UserLogin;

public sealed class UserLoginCommandValidation : AbstractValidator<UserLoginCommand>
{
    public UserLoginCommandValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}
