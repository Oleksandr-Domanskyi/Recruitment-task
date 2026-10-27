using Comment.Application.CQRS.Command.UpdateCommentRaiting;
using FluentValidation;

namespace Comment.Application.CQRS.Command.UpdateCommentRating;

public sealed class UpdateCommentRatingCommandValidator
    : AbstractValidator<UpdateCommentRatingCommand>
{
    public UpdateCommentRatingCommandValidator()
    {
        RuleFor(x => x.CommentId)
            .NotEmpty()
            .WithMessage("Comment ID cannot be empty.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID cannot be empty.");

        RuleFor(x => x.Value)
            .Must(value => value is -1 or 0 or 1)
            .WithMessage("Vote must be -1, 0 or 1.");
    }
}