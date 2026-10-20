
using FluentResults;
using MediatR;
using User.Application.DTO.Responses;

namespace User.Application.CQRS.Command.UserRegister;

public sealed record UserRegisterCommand(string UserName, string Email, string Password) : IRequest<Result<UserResponse>>;
