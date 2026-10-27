
using Comment.API.Hubs;
using Comment.Application.Interfaces;
using Comment.Core.Events;
using Microsoft.AspNetCore.SignalR;

namespace Comment.API.Realtime;

public sealed class SignalRCommentNotifier : ICommentNotifier
{
    private readonly IHubContext<CommentHub> _hubContext;

    public SignalRCommentNotifier(
        IHubContext<CommentHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyCreatedAsync(
        CommentCreatedEvent commentEvent,
        CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.All.SendAsync(
            "CommentCreated",
            commentEvent,
            cancellationToken
        );
    }

    public Task NotifyRatingChangedAsync(
        CommentRatingChangedEvent commentEvent,
        CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.All.SendAsync(
            "CommentRatingChanged",
            commentEvent,
            cancellationToken
        );
    }
}
