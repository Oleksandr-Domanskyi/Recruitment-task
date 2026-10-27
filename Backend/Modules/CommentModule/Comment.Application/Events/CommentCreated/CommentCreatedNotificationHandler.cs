using Comment.Application.Events.CommentCreated;
using Comment.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Comment.Application.Events.Handlers;

public sealed class CommentCreatedNotificationHandler
    : INotificationHandler<CommentCreatedNotification>
{
    private readonly ICommentEventQueue _eventQueue;
    private readonly ICommentCache _cache;
    private readonly ILogger<CommentCreatedNotificationHandler> _logger;

    public CommentCreatedNotificationHandler(
        ICommentEventQueue eventQueue,
        ICommentCache cache,
        ILogger<CommentCreatedNotificationHandler> logger)
    {
        _eventQueue = eventQueue;
        _cache = cache;
        _logger = logger;
    }

    public async Task Handle(
        CommentCreatedNotification notification,
        CancellationToken cancellationToken)
    {
        _cache.Invalidate();

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
                "Comment event enqueueing cancelled for {CommentId}.",
                notification.Event.CommentId
            );
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Failed to enqueue comment event for {CommentId}.",
                notification.Event.CommentId
            );
        }
    }
}
