
using FluentValidation;

namespace Comment.Application.CQRS.Common.CreateComment;

public sealed class CreateCommentCommandValidation
    : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidation()
    {
        RuleFor(x => x.Request)
            .NotNull()
            .WithMessage("Comment request is required.");

        When(x => x.Request is not null, () =>
        {
            RuleFor(x => x.Request.Text)
                .NotEmpty()
                .WithMessage("Comment text is required.");

            RuleFor(x => x.Request.UserId)
                .Must(id => !id.HasValue || id.Value != Guid.Empty)
                .WithMessage("Invalid user ID.");

            RuleFor(x => x.Request.ParentCommentId)
                .Must(id => !id.HasValue || id.Value != Guid.Empty)
                .WithMessage("Invalid parent comment ID.");

            RuleFor(x => x.Request.ImageUrl)
                .Must(url =>
                    string.IsNullOrWhiteSpace(url) ||
                    (Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                     (uri.Scheme == Uri.UriSchemeHttp ||
                      uri.Scheme == Uri.UriSchemeHttps)))
                .WithMessage("Image URL must be a valid HTTP or HTTPS URL.");
        });
    }
}
