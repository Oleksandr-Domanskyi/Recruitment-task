
using FluentResults;
using MediatR;
using User.Application.DTO.Responses;
using User.Application.Interfaces;
using User.Core.Entity;

namespace User.Application.CQRS.Command.UserRegister;

public sealed class UserRegisterCommandHandler(
    IUserRepositoryService userRepositoryService)
    : IRequestHandler<UserRegisterCommand, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(
        UserRegisterCommand request,
        CancellationToken cancellationToken)
    {
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            Email = request.Email
        };

        var result = await userRepositoryService.RegisterAsync(
            user,
            request.Password,
            cancellationToken);

        if (result.IsFailed)
            return Result.Fail<UserResponse>(result.Errors);

        var response = new UserResponse(
            user.Id,
            user.UserName!,
            user.Email!,
            user.AvatarUrl,
            user.CreatedAt);

        return Result.Ok(response);
    }
}
