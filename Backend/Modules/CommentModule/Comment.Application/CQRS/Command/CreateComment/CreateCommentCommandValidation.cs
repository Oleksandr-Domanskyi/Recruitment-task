
using FluentValidation;

namespace Comment.Application.CQRS.Common.CreateComment;

public sealed class CreateCommentCommandValidation : AbstractValidator<CreateCommentCommand>
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

            RuleFor(x => x.Request.FileName)
                .NotEmpty()
                .When(x => x.Request.File is not null)
                .WithMessage("File name is required.");

            RuleFor(x => x.Request.File)
                .NotNull()
                .When(x => !string.IsNullOrWhiteSpace(x.Request.FileName))
                .WithMessage("File content is required.");
        });
    }
}
