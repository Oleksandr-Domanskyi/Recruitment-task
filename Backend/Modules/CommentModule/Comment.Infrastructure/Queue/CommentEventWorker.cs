
using Comment.Application.Interfaces;
using Comment.Core.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Comment.Infrastructure.Queue;

public sealed class CommentEventWorker : BackgroundService
{
    private readonly ICommentEventQueue _queue;
    private readonly ICommentNotifier _notifier;
    private readonly ILogger<CommentEventWorker> _logger;

    public CommentEventWorker(
        ICommentEventQueue queue,
        ICommentNotifier notifier,
        ILogger<CommentEventWorker> logger)
    {
        _queue = queue;
        _notifier = notifier;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Comment event worker started."
        );

        try
        {
            await foreach (
                var commentEvent in _queue.ReadAllAsync(stoppingToken))
            {
                try
                {
                    switch (commentEvent)
                    {
                        case CommentCreatedEvent created:
                            await _notifier.NotifyCreatedAsync(
                                created,
                                stoppingToken
                            );
                            break;

                        case CommentRatingChangedEvent ratingChanged:
                            await _notifier.NotifyRatingChangedAsync(
                                ratingChanged,
                                stoppingToken
                            );
                            break;

                        default:
                            _logger.LogWarning(
                                "Unknown comment event type: {EventType}",
                                commentEvent.GetType().Name
                            );
                            continue;
                    }

                    _logger.LogInformation(
                        "Processed {EventType} for comment {CommentId}",
                        commentEvent.GetType().Name,
                        commentEvent switch
                        {
                            CommentCreatedEvent created => created.CommentId,
                            CommentRatingChangedEvent changed => changed.CommentId,
                            _ => Guid.Empty
                        }
                    );
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    _logger.LogError(
                        exception,
                        "Failed to process comment event {EventType}",
                        commentEvent.GetType().Name
                    );
                }
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
        }

        _logger.LogInformation(
            "Comment event worker stopped."
        );
    }
}
