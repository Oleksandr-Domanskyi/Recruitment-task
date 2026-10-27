
using Comment.Core.Events;

namespace Comment.Application.Interfaces;

public interface ICommentNotifier
{
    Task NotifyCreatedAsync(
        CommentCreatedEvent commentEvent,
        CancellationToken cancellationToken = default
    );

    Task NotifyRatingChangedAsync(
        CommentRatingChangedEvent commentEvent,
        CancellationToken cancellationToken = default
    );
}