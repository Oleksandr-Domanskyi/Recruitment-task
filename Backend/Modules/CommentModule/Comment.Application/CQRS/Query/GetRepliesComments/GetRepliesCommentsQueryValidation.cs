
using Comment.Application.CQRS.Query.GetRepliesComment;
using FluentValidation;

namespace Comment.Application.CQRS.Query.GetRepliesComments;

public sealed class GetRepliesCommentsQueryValidation
    : AbstractValidator<GetRepliesCommentsQuery>
{
    public GetRepliesCommentsQueryValidation()
    {
        RuleFor(x => x.ParentCommentId)
            .NotEmpty()
            .WithMessage("Parent comment ID cannot be empty.");
    }
}
