
using Comment.API.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Comment.API.EndPoints;

public static class CommentRealtimeEndpoints
{
    public static IEndpointRouteBuilder MapCommentRealtime(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<CommentHub>("/hubs/comments");

        return endpoints;
    }
}
