using AzureStorage.Configuration;
using Comment.Application.DTO;
using Comment.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using SPA.Shared.Abstractions.AzureStorage;


namespace Comment.Application.CQRS.Query.GetCommentById;

public sealed class GetCommentByIdQueryHandler
    : IRequestHandler<GetCommentByIdQuery, Result<CommentDto>>
{
    private readonly ICommentRepositoryService _commentRepositoryService;
    private readonly ICommentAttachmentStorage _attachmentStorage;
    private readonly IBlobStorageService _blobStorageService;
    private readonly AzureStorageOptions _storageOptions;

    public GetCommentByIdQueryHandler(
        ICommentRepositoryService commentRepositoryService,
        ICommentAttachmentStorage attachmentStorage,
        IBlobStorageService blobStorageService,
        IOptions<AzureStorageOptions> storageOptions)
    {
        _commentRepositoryService = commentRepositoryService;
        _attachmentStorage = attachmentStorage;
        _blobStorageService = blobStorageService;
        _storageOptions = storageOptions.Value;
    }

    public async Task<Result<CommentDto>> Handle(
        GetCommentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _commentRepositoryService.GetCommentByIdAsync(
            request.id,
            cancellationToken
        );

        if (result.IsFailed)
        {
            return Result.Fail<CommentDto>(result.Errors);
        }

        var item = result.Value;

        var avatarUrl = string.IsNullOrWhiteSpace(item.AvatarBlobName)
            ? null
            : _blobStorageService.GetBlobUrl(
                _storageOptions.AvatarsContainer,
                item.AvatarBlobName
            );

        var commentDto = new CommentDto(
            item.Comment,
            _attachmentStorage.GetBlobUrl,
            item.RepliesCount,
            item.UserVote,
            avatarUrl
        ) with
        {
            UserName = item.UserName ?? item.Comment.UserName
        };

        return Result.Ok(commentDto);
    }
}
