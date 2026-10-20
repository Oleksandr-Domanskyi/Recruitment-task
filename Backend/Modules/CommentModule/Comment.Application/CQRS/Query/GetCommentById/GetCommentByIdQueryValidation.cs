
using FluentValidation;

namespace Comment.Application.CQRS.Query.GetCommentById;

public sealed class GetCommentByIdQueryValidation
    : AbstractValidator<GetCommentByIdQuery>
{
    public GetCommentByIdQueryValidation()
    {
        RuleFor(x => x.id)
            .NotEmpty()
            .WithMessage("Comment ID cannot be empty.");
    }
}
