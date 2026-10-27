
using Comment.Application.DTO.ReadModels;
using Comment.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Query.GetUserDiscussions;

public sealed class GetUserDiscussionsQueryHandler
    : IRequestHandler<
        GetUserDiscussionsQuery,
        Result<List<UserDiscussionDto>>>
{
    private readonly ICommentRepositoryService _commentRepositoryService;

    public GetUserDiscussionsQueryHandler(
        ICommentRepositoryService commentRepositoryService)
    {
        _commentRepositoryService = commentRepositoryService;
    }

    public async Task<Result<List<UserDiscussionDto>>> Handle(
        GetUserDiscussionsQuery request,
        CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit, 1, 20);

        var result = await _commentRepositoryService.GetUserDiscussionsAsync(
            request.UserId,
            limit,
            cancellationToken
        );

        if (result.IsFailed)
            return Result.Fail<List<UserDiscussionDto>>(result.Errors);

        return Result.Ok(result.Value);
    }
}
