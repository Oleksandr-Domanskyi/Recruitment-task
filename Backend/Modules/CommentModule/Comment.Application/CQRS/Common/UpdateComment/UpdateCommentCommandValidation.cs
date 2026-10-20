
using FluentValidation;

namespace Comment.Application.CQRS.Common.UpdateComment;

public sealed class UpdateCommentCommandValidation
    : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidation()
    {
        RuleFor(x => x.ParrentId)
            .NotEmpty()
            .WithMessage("Comment ID cannot be empty.");

        RuleFor(x => x.UpdateCommentRequest)
            .NotNull()
            .WithMessage("Update comment request is required.");

        When(x => x.UpdateCommentRequest is not null, () =>
        {
            RuleFor(x => x.UpdateCommentRequest.Text)
                .NotEmpty()
                .WithMessage("Comment text cannot be empty.");

            RuleFor(x => x.UpdateCommentRequest.ImageUrl)
                .Must(url =>
                    string.IsNullOrWhiteSpace(url) ||
                    (Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                     (uri.Scheme == Uri.UriSchemeHttp ||
                      uri.Scheme == Uri.UriSchemeHttps)))
                .WithMessage("Image URL must be a valid HTTP or HTTPS URL.");
        });
    }
}
