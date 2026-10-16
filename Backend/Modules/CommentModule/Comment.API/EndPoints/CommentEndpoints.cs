using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Comment.API.Routes;
using Asp.Versioning;

namespace Comment.API.EndPoints;

public static class CommentEndpoints
{
    public static IEndpointRouteBuilder MapCommentEndpoints(
        this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();


        var group = app.MapGroup(CommentRoutes.Base)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(new ApiVersion(1, 0))
            .WithTags(CommentRoutes.Tag);

        group.MapGet(CommentRoutes.Root, GetComments);
        group.MapGet(CommentRoutes.ById, GetCommentById);
        group.MapPut(CommentRoutes.ById, UpdateComment);
        group.MapPost(CommentRoutes.Root, CreateComment);
        group.MapDelete(CommentRoutes.ById, DeleteComment);
        group.MapPatch(CommentRoutes.Rating, UpdateCommentRating);

        return app;
    }

    private static IResult GetComments()
    {
        return Results.Ok();
    }

    private static IResult GetCommentById(Guid id)
    {
        return Results.Ok(id);
    }

    private static IResult CreateComment()
    {
        return Results.Created();
    }

    private static IResult UpdateComment(Guid id)
    {
        return Results.Ok(id);
    }
    public static IResult UpdateCommentRating(Guid id, int rating)
    {
        return Results.Ok(id);
    }

    private static IResult DeleteComment(Guid id)
    {
        return Results.NoContent();
    }
}