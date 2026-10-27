
using Comment.Core.Events;

namespace Comment.Application.Interfaces;

public interface ICommentEventQueue
{
    ValueTask EnqueueAsync(
        ICommentRealtimeEvent commentEvent,
        CancellationToken cancellationToken = default
    );

    IAsyncEnumerable<ICommentRealtimeEvent> ReadAllAsync(
        CancellationToken cancellationToken = default
    );
}
