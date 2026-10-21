
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;

using Asp.Versioning;
using MediatR;

using Comment.API.Routes;
using Comment.Application.Filters;
using Comment.Application.CQRS.Query.GetComments;
using Comment.Application.CQRS.Query.GetCommentById;
using Comment.Application.CQRS.Common.CreateComment;
using Comment.Application.DTO;
using Comment.Application.CQRS.Common.UpdateCommentRaiting;
using Comment.Application.CQRS.Common.UpdateComment;
using Comment.Application.CQRS.Common.DeleteComment;
using Comment.Application.CQRS.Query.GetRepliesComment;
using SPA.Shared.Constants;
using Microsoft.AspNetCore.Mvc;
using Comment.API.Forms;

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
        group.MapGet(CommentRoutes.Replies, GetRepliesComments);
        group.MapPut(CommentRoutes.ById, UpdateComment);
        group.MapPost(CommentRoutes.Root, CreateComment)
            .Accepts<IFormFile>("multipart/form-data")
            .RequireRateLimiting(RateLimitPolicies.Comments);
        group.MapDelete(CommentRoutes.ById, DeleteComment);
        group.MapPatch(CommentRoutes.Rating, UpdateCommentRating);

        return app;
    }

    private static async Task<IResult> GetComments(
        [AsParameters] CommentFilters filters,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            new GetCommentsQuery(filters),
            cancellationToken);

        return response.IsFailed
            ? Results.BadRequest(response.Errors)
            : Results.Ok(response.Value);
    }

    private static async Task<IResult> GetCommentById(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            new GetCommentByIdQuery(id),
            cancellationToken);

        return response.IsFailed
            ? Results.NotFound(response.Errors)
            : Results.Ok(response.Value);
    }
    private static async Task<IResult> GetRepliesComments(
        Guid parentCommentId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            new GetRepliesCommentsQuery(parentCommentId),
            cancellationToken);

        if (response.IsFailed)
            return Results.BadRequest(response.Errors);

        return Results.Ok(response.Value);
    }

    private static async Task<IResult> CreateComment(
        [FromForm] CreateCommentForm form,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        await using var stream = form.File?.OpenReadStream();

        var response = await mediator.Send(
            new CreateCommentCommand(form.ToRequest(stream)),
            cancellationToken);

        return response.IsFailed
            ? Results.BadRequest(response.Errors)
            : Results.Ok(response.Value);
    }

    private static async Task<IResult> UpdateComment(
        Guid id,
        [FromForm] UpdateCommentForm form,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        await using var stream = form.File?.OpenReadStream();

        var response = await mediator.Send(
            new UpdateCommentCommand(id, form.ToRequest(stream)),
            cancellationToken);

        return response.IsFailed
            ? Results.NotFound(response.Errors)
            : Results.Ok(response.Value);
    }

    private static async Task<IResult> UpdateCommentRating(
        Guid id,
        int rating,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
          new UpdateCommentRaitingCommand(id, rating),
          cancellationToken);

        return response.IsFailed
          ? Results.NotFound(response.Errors)
          : Results.Ok(response.Value);
    }

    private static async Task<IResult> DeleteComment(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
          new DeleteCommentCommand(id),
          cancellationToken);

        return response.IsFailed
            ? Results.NotFound(response.Errors)
            : Results.Ok();
    }
}
