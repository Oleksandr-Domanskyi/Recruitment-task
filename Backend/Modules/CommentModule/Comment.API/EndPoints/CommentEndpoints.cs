
using System.Security.Claims;

using Asp.Versioning;
using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using Comment.API.Forms;
using Comment.API.Routes;
using Comment.Application.Filters;

using Comment.Application.CQRS.Command.CreateComment;
using Comment.Application.CQRS.Command.DeleteComment;
using Comment.Application.CQRS.Command.UpdateComment;
using Comment.Application.CQRS.Command.UpdateCommentRaiting;

using Comment.Application.CQRS.Query.GetCommentById;
using Comment.Application.CQRS.Query.GetComments;
using Comment.Application.CQRS.Query.GetRepliesComment;

using SPA.Shared.Constants;
using Comment.Application.DTO;
using Comment.Application.CQRS.Query.GetUserDiscussions;

namespace Comment.API.EndPoints;

public static class CommentEndpoints
{
    private const string AdminRole = "Admin";

    public static IEndpointRouteBuilder MapCommentEndpoints(
        this IEndpointRouteBuilder app)
    {
        var version = new ApiVersion(1, 0);

        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(version)
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup(CommentRoutes.Base)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(version)
            .WithTags(CommentRoutes.Tag);

        group.MapGet(CommentRoutes.Root, GetComments);
        group.MapGet(CommentRoutes.ById, GetCommentById);
        group.MapGet(CommentRoutes.Replies, GetRepliesComments);
        group.MapGet(
        CommentRoutes.UserDiscussions, GetUserDiscussions)
            .RequireAuthorization();
        group.MapPost(CommentRoutes.Root, CreateComment)
            .RequireRateLimiting(RateLimitPolicies.Comments)
            .DisableAntiforgery();
        group.MapPut(CommentRoutes.ById, UpdateComment)
            .RequireAuthorization(policy =>
                policy.RequireRole(AdminRole))
            .DisableAntiforgery();
        group.MapDelete(CommentRoutes.ById, DeleteComment)
            .RequireAuthorization(policy =>
                policy.RequireRole(AdminRole));
        group.MapPatch(CommentRoutes.Rating, UpdateCommentRating)
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> GetComments(
        [AsParameters] CommentFilters filters,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetCommentsQuery(filters),
            cancellationToken);

        return result.IsFailed
            ? Results.BadRequest(result.Errors)
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> GetCommentById(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetCommentByIdQuery(id),
            cancellationToken);

        return result.IsFailed
            ? Results.NotFound(result.Errors)
            : Results.Ok(result.Value);
    }


    private static async Task<IResult> GetRepliesComments(
        Guid parentCommentId,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        Guid? userId = Guid.TryParse(
            user.FindFirstValue(ClaimTypes.NameIdentifier),
            out var parsedUserId)
                ? parsedUserId
                : null;

        var result = await sender.Send(
            new GetRepliesCommentsQuery(parentCommentId, userId),
            cancellationToken);

        return result.IsFailed
            ? Results.BadRequest(result.Errors)
            : Results.Ok(result.Value);
    }


    private static async Task<IResult> GetUserDiscussions(
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken,
        [FromQuery] int limit = 5)
    {
        var userIdClaim = user.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Results.Unauthorized();

        var result = await sender.Send(
            new GetUserDiscussionsQuery(
                userId,
                limit
            ),
            cancellationToken
        );

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(
                result.Errors.Select(error => error.Message)
            );
    }

    private static async Task<IResult> CreateComment(
        [FromForm] CreateCommentForm form,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (!TryGetUser(user, out var userId, out var userName))
            return Results.Unauthorized();

        await using var stream = form.File?.OpenReadStream();

        var request = form.ToRequest(stream) with
        {
            UserId = userId,
            UserName = userName
        };

        var result = await sender.Send(
            new CreateCommentCommand(request),
            cancellationToken);

        return result.IsFailed
            ? Results.BadRequest(result.Errors)
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> UpdateComment(
        Guid id,
        [FromForm] UpdateCommentForm form,
        ISender sender,
        CancellationToken cancellationToken)
    {
        await using var stream = form.File?.OpenReadStream();

        var result = await sender.Send(
            new UpdateCommentCommand(
                id,
                form.ToRequest(stream)),
            cancellationToken);

        return result.IsFailed
            ? Results.NotFound(result.Errors)
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> UpdateCommentRating(
        Guid id,
        UpdateCommentRatingRequest request,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
            return Results.Unauthorized();

        var result = await sender.Send(
            new UpdateCommentRatingCommand(
                id,
                userId,
                request.Rating),
            cancellationToken);

        return result.IsFailed
            ? Results.BadRequest(result.Errors)
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteComment(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new DeleteCommentCommand(id),
            cancellationToken);

        return result.IsFailed
            ? Results.NotFound(result.Errors)
            : Results.NoContent();
    }

    private static bool TryGetUser(
        ClaimsPrincipal user,
        out Guid? userId,
        out string? userName)
    {
        userId = null;
        userName = null;

        if (user.Identity?.IsAuthenticated != true)
            return true;

        var idClaim = user.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(idClaim, out var id))
            return false;

        userId = id;

        userName = user.FindFirstValue(
            ClaimTypes.Name);

        return true;
    }
}
