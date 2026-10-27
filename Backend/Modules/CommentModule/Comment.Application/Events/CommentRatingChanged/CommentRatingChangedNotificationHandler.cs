using Comment.Application.Events.CommentRatingChanged;
using Comment.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Comment.Application.Events.Handlers;

public sealed class CommentRatingChangedNotificationHandler
    : INotificationHandler<CommentRatingChangedNotification>
{
    private readonly ICommentEventQueue _eventQueue;
    private readonly ILogger<CommentRatingChangedNotificationHandler> _logger;

    public CommentRatingChangedNotificationHandler(
        ICommentEventQueue eventQueue,
        ILogger<CommentRatingChangedNotificationHandler> logger)
    {
        _eventQueue = eventQueue;
        _logger = logger;
    }

    public async Task Handle(
        CommentRatingChangedNotification notification,
        CancellationToken cancellationToken)
    {
        try
        {
            await _eventQueue.EnqueueAsync(
                notification.Event,
                cancellationToken
            );
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(
                "Rating event enqueueing cancelled for comment {CommentId}.",
                notification.Event.CommentId
            );
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Failed to enqueue rating event for comment {CommentId}.",
                notification.Event.CommentId
            );
        }
    }
}
