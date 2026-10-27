
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Comment.API.Hubs;

public sealed class CommentHub : Hub
{
    private readonly ILogger<CommentHub> _logger;

    public CommentHub(ILogger<CommentHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation(
            "Client connected to CommentHub: {ConnectionId}",
            Context.ConnectionId
        );

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(
        Exception? exception)
    {
        _logger.LogInformation(
            "Client disconnected from CommentHub: {ConnectionId}",
            Context.ConnectionId
        );

        await base.OnDisconnectedAsync(exception);
    }
}
