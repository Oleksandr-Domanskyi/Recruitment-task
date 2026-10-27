
using AzureStorage.Configuration;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using SPA.Shared.Abstractions.AzureStorage;
using User.Application.DTO.Responses;
using User.Application.Interfaces;

namespace User.Application.CQRS.Query.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserResponse>>
{
    private readonly IUserRepositoryService _userRepositoryService;
    private readonly IBlobStorageService _blobStorageService;
    private readonly AzureStorageOptions _options;

    public GetCurrentUserQueryHandler(
        IUserRepositoryService userRepositoryService,
        IBlobStorageService blobStorageService,
        IOptions<AzureStorageOptions> options)
    {
        _userRepositoryService = userRepositoryService;
        _blobStorageService = blobStorageService;
        _options = options.Value;
    }

    public async Task<Result<UserResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userResult = await _userRepositoryService.GetByIdAsync(
            request.UserId,
            cancellationToken
        );

        if (userResult.IsFailed)
            return Result.Fail<UserResponse>(userResult.Errors);

        var user = userResult.Value;

        var response = new UserResponse(
            user,
            name => _blobStorageService.GetBlobUrl(
                _options.AvatarsContainer,
                name
            )
        );

        return Result.Ok(response);
    }
}
