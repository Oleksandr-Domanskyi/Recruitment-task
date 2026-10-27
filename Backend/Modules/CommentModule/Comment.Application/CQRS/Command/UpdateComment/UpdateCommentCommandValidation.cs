
using FluentValidation;

namespace Comment.Application.CQRS.Command.UpdateComment;

public sealed class UpdateCommentCommandValidation : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidation()
    {
        RuleFor(x => x.CommentId)
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

            RuleFor(x => x.UpdateCommentRequest)
                .Must(x => x.File is null || !x.RemoveAttachment)
                .WithMessage("Cannot upload and remove an attachment simultaneously.");

            RuleFor(x => x.UpdateCommentRequest.FileName)
                .NotEmpty()
                .When(x => x.UpdateCommentRequest.File is not null)
                .WithMessage("File name is required.");
        });
    }
}
