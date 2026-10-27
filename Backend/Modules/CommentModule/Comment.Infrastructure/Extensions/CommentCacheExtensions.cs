
using Comment.Application.Interfaces;
using Comment.Infrastructure.Cache;
using Microsoft.Extensions.DependencyInjection;

namespace Comment.Infrastructure.Extensions;

public static class CommentCacheExtensions
{
    public static void AddCommentCache(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICommentCache, CommentCache>();
    }
}
