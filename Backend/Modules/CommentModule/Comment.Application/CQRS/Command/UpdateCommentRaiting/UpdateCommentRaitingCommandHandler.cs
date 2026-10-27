using Comment.Application.CQRS.Command.UpdateCommentRaiting;
using Comment.Application.Events.CommentRatingChanged;
using Comment.Application.Interfaces;
using Comment.Core.Events;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Command.UpdateCommentRating;

public sealed class UpdateCommentRatingCommandHandler
    : IRequestHandler<UpdateCommentRatingCommand, Result<int>>
{
    private readonly ICommentRepositoryService _commentRepositoryService;
    private readonly IPublisher _publisher;

    public UpdateCommentRatingCommandHandler(
        ICommentRepositoryService commentRepositoryService,
        IPublisher publisher)
    {
        _commentRepositoryService = commentRepositoryService;
        _publisher = publisher;
    }

    public async Task<Result<int>> Handle(
        UpdateCommentRatingCommand request,
        CancellationToken cancellationToken)
    {
        var response = await _commentRepositoryService.UpdateCommentRatingAsync(
            request.CommentId,
            request.UserId,
            request.Value,
            cancellationToken);

        if (response.IsFailed)
        {
            return Result.Fail<int>(response.Errors);
        }

        await _publisher.Publish(
            new CommentRatingChangedNotification(
                new CommentRatingChangedEvent(
                    request.CommentId,
                    response.Value
                )
            ),
            cancellationToken);

        return Result.Ok(response.Value);
    }
}
