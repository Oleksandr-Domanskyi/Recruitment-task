using Comment.Application.Interfaces;
using Comment.Infrastructure.Queue;
using Microsoft.Extensions.DependencyInjection;

namespace Comment.Infrastructure.Extensions;

public static class CommentQueueExtensions
{
    public static void AddCommentQueue(
        this IServiceCollection services)
    {
        services.AddSingleton<
            ICommentEventQueue,
            CommentEventQueue>();

        services.AddHostedService<CommentEventWorker>();

    }
}