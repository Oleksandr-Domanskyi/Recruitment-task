
using System.Threading;
using Comment.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Comment.Infrastructure.Cache;

public sealed class CommentCache : ICommentCache
{
    private static readonly TimeSpan CacheLifetime =
        TimeSpan.FromSeconds(30);

    private readonly IMemoryCache _cache;
    private readonly ILogger<CommentCache> _logger;

    private long _version;

    public CommentCache(
        IMemoryCache cache,
        ILogger<CommentCache> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken = default,
        Func<T, bool>? shouldCache = null)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var version = Interlocked.Read(ref _version);
        var cacheKey = $"comments:v{version}:{key}";

        if (_cache.TryGetValue(cacheKey, out T? cached))
        {
            _logger.LogDebug(
                "Comment cache HIT: {CacheKey}",
                cacheKey
            );

            return cached!;
        }

        _logger.LogDebug(
            "Comment cache MISS: {CacheKey}",
            cacheKey
        );

        var result = await factory(cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        var canCache =
            shouldCache is null || shouldCache(result);

        if (
            canCache &&
            Interlocked.Read(ref _version) == version
        )
        {
            _cache.Set(
                cacheKey,
                result,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        CacheLifetime
                }
            );
        }

        return result;
    }

    public void Invalidate()
    {
        var version = Interlocked.Increment(
            ref _version
        );

        _logger.LogDebug(
            "Comment cache invalidated. Version: {Version}",
            version
        );
    }
}
