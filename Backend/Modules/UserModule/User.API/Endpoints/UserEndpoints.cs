
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
using User.Application.DTO.Requests;

namespace User.API.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(UserRoutes.Base)
            .WithTags("Users")
            .WithApiVersionSet(
                app.NewApiVersionSet()
                    .HasApiVersion(new ApiVersion(1, 0))
                    .Build());

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
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors.Select(x => x.Message));
    }

    private static async Task<IResult> Login(
        UserLoginCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Unauthorized();
    }

    private static async Task<IResult> Refresh(
        RefreshTokenCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Unauthorized();
    }

    private static async Task<IResult> Logout(
        LogoutUserRequest request,
        ClaimsPrincipal user,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var id))
            return Results.Unauthorized();

        var result = await mediator.Send(new LogoutUserCommand(request.RefreshToken, id), cancellationToken);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(result.Errors.Select(x => x.Message));
    }

    private static async Task<IResult> UpdateAvatar(
        [FromForm] UpdateUserAvatarForm form,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var id))
            return Results.Unauthorized();

        using var stream = form.File.OpenReadStream();

        var command = new UpdateUserAvatarCommand(id, stream, form.File.FileName);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors.Select(x => x.Message));
    }
}
