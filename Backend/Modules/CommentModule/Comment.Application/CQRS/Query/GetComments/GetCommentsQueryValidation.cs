
using FluentValidation;

namespace Comment.Application.CQRS.Query.GetComments;

public sealed class GetCommentsQueryValidation
    : AbstractValidator<GetCommentsQuery>
{
    public GetCommentsQueryValidation()
    {
        RuleFor(x => x.Filter)
            .NotNull()
            .WithMessage("Comment filters are required.");

        When(x => x.Filter is not null, () =>
        {
            RuleFor(x => x.Filter.Page)
                .GreaterThan(0)
                .WithMessage("Page must be greater than zero.");

            RuleFor(x => x.Filter.SortBy)
                .IsInEnum()
                .WithMessage("Invalid sorting field.");

            RuleFor(x => x.Filter.Direction)
                .IsInEnum()
                .WithMessage("Invalid sorting direction.");
        });
    }
}
