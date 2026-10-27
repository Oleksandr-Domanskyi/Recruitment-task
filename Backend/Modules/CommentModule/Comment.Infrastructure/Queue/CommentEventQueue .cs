
using System.Threading.Channels;
using Comment.Application.Interfaces;
using Comment.Core.Events;

namespace Comment.Infrastructure.Queue;

public sealed class CommentEventQueue : ICommentEventQueue
{
    private readonly Channel<ICommentRealtimeEvent> _channel;

    public CommentEventQueue()
    {
        var options = new BoundedChannelOptions(256)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        };

        _channel = Channel.CreateBounded<ICommentRealtimeEvent>(
            options
        );
    }

    public ValueTask EnqueueAsync(
        ICommentRealtimeEvent commentEvent,
        CancellationToken cancellationToken = default)
    {
        return _channel.Writer.WriteAsync(
            commentEvent,
            cancellationToken
        );
    }

    public IAsyncEnumerable<ICommentRealtimeEvent> ReadAllAsync(
        CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(
            cancellationToken
        );
    }
}
