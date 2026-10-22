
using AzureStorage.Configuration;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using Shared.Constants;
using Shared.Files;
using SPA.Shared.Abstractions.AzureStorage;
using User.Application.DTO.Responses;
using User.Application.Interfaces;

namespace User.Application.CQRS.Command.UpdateUserAvatar;

public sealed class UpdateUserAvatarCommandHandler
    : IRequestHandler<UpdateUserAvatarCommand, Result<UserResponse>>
{
    private readonly IUserRepositoryService _userRepositoryService;
    private readonly IBlobStorageService _blobStorageService;
    private readonly AzureStorageOptions _options;

    public UpdateUserAvatarCommandHandler(
        IUserRepositoryService userRepositoryService,
        IBlobStorageService blobStorageService,
        IOptions<AzureStorageOptions> options)
    {
        _userRepositoryService = userRepositoryService;
        _blobStorageService = blobStorageService;
        _options = options.Value;
    }

    public async Task<Result<UserResponse>> Handle(UpdateUserAvatarCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _userRepositoryService.GetByIdAsync(request.UserId, cancellationToken);

        if (userResult.IsFailed)
            return Result.Fail<UserResponse>(userResult.Errors);

        var user = userResult.Value;

        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        var image = ImageProcessor.Process(request.File, extension);

        var blobName = $"{Guid.NewGuid()}{extension}";
        var oldAvatar = user.AvatarUrl;

        using var stream = new MemoryStream(image);

        await _blobStorageService.UploadAsync(
            stream,
            _options.AvatarsContainer,
            blobName,
            ImageFileConstants.GetContentType(extension),
            cancellationToken);

        user.UpdateAvatar(blobName);

        var result = await _userRepositoryService.UpdateAsync(user, cancellationToken);

        if (result.IsFailed)
        {
            await _blobStorageService.DeleteAsync(_options.AvatarsContainer, blobName, cancellationToken);

            return Result.Fail<UserResponse>(result.Errors);
        }

        if (!string.IsNullOrWhiteSpace(oldAvatar))
        {
            await _blobStorageService.DeleteAsync(_options.AvatarsContainer, oldAvatar, cancellationToken);
        }

        return Result.Ok(new UserResponse(user, name => _blobStorageService.GetBlobUrl(_options.AvatarsContainer, name)));
    }
}
