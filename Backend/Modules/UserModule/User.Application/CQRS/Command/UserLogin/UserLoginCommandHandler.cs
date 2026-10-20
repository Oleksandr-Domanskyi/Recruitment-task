
using FluentResults;
using MediatR;
using User.Application.DTO.Responses;
using User.Application.Interfaces;

namespace User.Application.CQRS.Command.UserLogin;

public sealed class UserLoginCommandHandler(
    IUserRepositoryService userRepositoryService,
    IRefreshTokenService refreshTokenService)
    : IRequestHandler<UserLoginCommand, Result<AuthTokenResponse>>
{
    public async Task<Result<AuthTokenResponse>> Handle(
        UserLoginCommand request,
        CancellationToken cancellationToken)
    {
        var userResult = await userRepositoryService.GetByEmailAsync(
            request.Email,
            cancellationToken);

        if (userResult.IsFailed)
            return Result.Fail<AuthTokenResponse>("Invalid credentials.");

        var loginResult = await userRepositoryService.LoginAsync(
            userResult.Value,
            request.Password,
            cancellationToken);

        if (loginResult.IsFailed)
            return Result.Fail<AuthTokenResponse>("Invalid credentials.");

        return await refreshTokenService.CreateSessionAsync(
            userResult.Value,
            cancellationToken);
    }
}
