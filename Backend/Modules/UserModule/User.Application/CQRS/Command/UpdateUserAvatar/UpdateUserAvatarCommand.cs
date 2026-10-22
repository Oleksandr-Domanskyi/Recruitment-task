using FluentResults;
using MediatR;
using User.Application.DTO.Responses;

namespace User.Application.CQRS.Command.UpdateUserAvatar;

public sealed record UpdateUserAvatarCommand(
    Guid UserId,
    Stream File,
    string FileName
) : IRequest<Result<UserResponse>>;