using Comment.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Common.UpdateCommentRaiting;

public class UpdateCommentRaitingCommandHandler
    : IRequestHandler<UpdateCommentRaitingCommand, Result<int>>
{
    private readonly ICommentRepositoryService _commentRepositoryService;

    public UpdateCommentRaitingCommandHandler(
        ICommentRepositoryService commentRepositoryService)
    {
        _commentRepositoryService = commentRepositoryService;
    }

    public async Task<Result<int>> Handle(
        UpdateCommentRaitingCommand request,
        CancellationToken cancellationToken)
    {
        var response = await _commentRepositoryService.UpdateCommentRatingAsync(
            request.ParrentCommentId,
            request.Raiting,
            cancellationToken);

        if (response.IsFailed)
            return Result.Fail<int>(response.Errors);

        return Result.Ok(response.Value);
    }
}