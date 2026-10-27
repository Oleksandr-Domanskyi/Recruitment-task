
using System.Security.Claims;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SPA.Shared.Constants;
using User.API.Forms;
using User.API.Routes;
using User.Application.CQRS.Command.LogoutUser;
using User.Application.CQRS.Command.RefreshToken;
using User.Application.CQRS.Command.UpdateUserAvatar;
using User.Application.CQRS.Command.UserLogin;
using User.Application.CQRS.Command.UserRegister;
using User.Application.CQRS.Query.GetCurrentUser;

namespace User.API.Endpoints;

public static class UserEndpoints
{
    private const string RefreshTokenCookie = "refreshToken";

    public static IEndpointRouteBuilder MapUserEndpoints(
        this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .Build();

        var group = app.MapGroup(UserRoutes.Base)
            .WithTags("Users")
            .WithApiVersionSet(versionSet);

        group.MapGet(UserRoutes.Me, GetCurrentUser)
            .RequireAuthorization();

        group.MapPost(UserRoutes.Register, Register)
            .RequireRateLimiting(RateLimitPolicies.Auth)
            .AllowAnonymous();

        group.MapPost(UserRoutes.Login, Login)
            .RequireRateLimiting(RateLimitPolicies.Auth)
            .AllowAnonymous();

        group.MapPost(UserRoutes.Refresh, Refresh)
            .RequireRateLimiting(RateLimitPolicies.Auth)
            .AllowAnonymous();

        group.MapPost(UserRoutes.Logout, Logout)
            .RequireAuthorization();

        group.MapPut(UserRoutes.Avatar, UpdateAvatar)
            .RequireAuthorization()
            .DisableAntiforgery();

        return app;
    }

    private static async Task<IResult> Register(
        UserRegisterCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(
                result.Errors.Select(error => error.Message)
            );
    }

    private static async Task<IResult> GetCurrentUser(
    ClaimsPrincipal user,
    ISender sender,
    CancellationToken cancellationToken)
    {
        if (!TryGetUserId(user, out var userId))
            return Results.Unauthorized();

        var result = await sender.Send(
            new GetCurrentUserQuery(userId),
            cancellationToken
        );

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound();
    }

    private static async Task<IResult> Login(
        UserLoginCommand command,
        ISender sender,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            command,
            cancellationToken
        );

        if (result.IsFailed)
            return Results.Unauthorized();

        var tokens = result.Value;

        SetRefreshTokenCookie(
            context,
            tokens.RefreshToken
        );

        return Results.Ok(new
        {
            tokens.AccessToken,
            tokens.ExpiresAt
        });
    }

    private static async Task<IResult> Refresh(
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var refreshToken = GetRefreshToken(context);

        if (string.IsNullOrWhiteSpace(refreshToken))
            return Results.Unauthorized();

        var result = await sender.Send(
            new RefreshTokenCommand(refreshToken),
            cancellationToken
        );

        if (result.IsFailed)
        {
            DeleteRefreshTokenCookie(context);

            return Results.Unauthorized();
        }

        var tokens = result.Value;

        SetRefreshTokenCookie(
            context,
            tokens.RefreshToken
        );

        return Results.Ok(new
        {
            tokens.AccessToken,
            tokens.ExpiresAt
        });
    }

    private static async Task<IResult> Logout(
        HttpContext context,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(user, out var userId))
            return Results.Unauthorized();

        var refreshToken = GetRefreshToken(context);

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            DeleteRefreshTokenCookie(context);

            return Results.Unauthorized();
        }

        var result = await sender.Send(
            new LogoutUserCommand(refreshToken, userId),
            cancellationToken
        );

        if (result.IsFailed)
        {
            return Results.BadRequest(
                result.Errors.Select(error => error.Message)
            );
        }

        DeleteRefreshTokenCookie(context);

        return Results.NoContent();
    }

    private static async Task<IResult> UpdateAvatar(
        [FromForm] UpdateUserAvatarForm form,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(user, out var userId))
            return Results.Unauthorized();

        using var stream = form.File.OpenReadStream();

        var command = new UpdateUserAvatarCommand(
            userId,
            stream,
            form.File.FileName
        );

        var result = await sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(
                result.Errors.Select(error => error.Message)
            );
    }

    private static bool TryGetUserId(
        ClaimsPrincipal user,
        out Guid userId)
    {
        var claim = user.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        return Guid.TryParse(claim, out userId);
    }

    private static string? GetRefreshToken(
        HttpContext context)
    {
        return context.Request.Cookies.TryGetValue(
            RefreshTokenCookie,
            out var refreshToken
        )
            ? refreshToken
            : null;
    }

    private static void SetRefreshTokenCookie(
        HttpContext context,
        string refreshToken)
    {
        context.Response.Cookies.Append(
            RefreshTokenCookie,
            refreshToken,
            RefreshCookieOptions.Create()
        );
    }

    private static void DeleteRefreshTokenCookie(
        HttpContext context)
    {
        context.Response.Cookies.Delete(
            RefreshTokenCookie,
            RefreshCookieOptions.Create()
        );
    }
}
