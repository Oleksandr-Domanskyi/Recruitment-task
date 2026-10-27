
using FluentValidation;

namespace Comment.Application.CQRS.Command.DeleteComment;

public sealed class DeleteCommentCommandValidation
    : AbstractValidator<DeleteCommentCommand>
{
    public DeleteCommentCommandValidation()
    {
        RuleFor(x => x.CommentId)
            .NotEmpty()
            .WithMessage("Comment ID cannot be empty.");
    }
}
