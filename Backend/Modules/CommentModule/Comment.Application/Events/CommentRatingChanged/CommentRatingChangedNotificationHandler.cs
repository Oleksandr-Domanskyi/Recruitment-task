using Comment.Application.Events.CommentRatingChanged;
using Comment.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Comment.Application.Events.Handlers;

public sealed class CommentRatingChangedNotificationHandler
    : INotificationHandler<CommentRatingChangedNotification>
{
    private readonly ICommentEventQueue _eventQueue;
    private readonly ICommentCache _cache;
    private readonly ILogger<CommentRatingChangedNotificationHandler> _logger;

    public CommentRatingChangedNotificationHandler(
        ICommentEventQueue eventQueue,
        ICommentCache cache,
        ILogger<CommentRatingChangedNotificationHandler> logger)
    {
        _eventQueue = eventQueue;
        _cache = cache;
        _logger = logger;
    }

    public async Task Handle(
        CommentRatingChangedNotification notification,
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
                "Rating event enqueueing cancelled for {CommentId}.",
                notification.Event.CommentId
            );
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Failed to enqueue rating event for {CommentId}.",
                notification.Event.CommentId
            );
        }
    }
}
