
using Comment.Application.Interfaces;
using Comment.Application.Events.CommentCreated;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Comment.Application.Events.CommentCreated;

public sealed class CommentCreatedNotificationHandler
    : INotificationHandler<CommentCreatedNotification>
{
    private readonly ICommentEventQueue _eventQueue;

    private readonly ILogger<CommentCreatedNotificationHandler> _logger;

    public CommentCreatedNotificationHandler(
        ICommentEventQueue eventQueue,
        ILogger<CommentCreatedNotificationHandler> logger)
    {
        _eventQueue = eventQueue;
        _logger = logger;
    }

    public async Task Handle(
        CommentCreatedNotification notification,
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
                "Realtime event enqueueing was cancelled for comment {CommentId}.",
                notification.Event.CommentId
            );
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Failed to enqueue realtime event for comment {CommentId}.",
                notification.Event.CommentId
            );
        }
    }
}
