
using Comment.API.Hubs;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Comment.API.Extensions;

public static class RealtimeExtensions
{
    public static void AddCommentRealtime(
        this IServiceCollection services)
    {
        services.AddSignalR();
    }
}
