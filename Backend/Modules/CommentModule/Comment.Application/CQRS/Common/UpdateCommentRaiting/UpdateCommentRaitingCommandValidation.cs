using FluentValidation;

namespace Comment.Application.CQRS.Common.UpdateCommentRaiting;

public sealed class UpdateCommentRaitingCommandValidation
    : AbstractValidator<UpdateCommentRaitingCommand>
{
    public UpdateCommentRaitingCommandValidation()
    {
        RuleFor(x => x.ParrentCommentId)
            .NotEmpty()
            .WithMessage("Comment ID cannot be empty.");

        RuleFor(x => x.Raiting)
            .Must(rating => rating is 1 or -1)
            .WithMessage("Rating must be either 1 or -1.");
    }
}