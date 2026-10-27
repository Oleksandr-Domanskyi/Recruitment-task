
using AzureStorage.Configuration;
using Comment.Application.DTO;
using Comment.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using SPA.Shared.Abstractions.AzureStorage;

namespace Comment.Application.CQRS.Query.GetRepliesComment;

public sealed class GetRepliesCommentsQueryHandler
    : IRequestHandler<GetRepliesCommentsQuery, Result<List<CommentDto>>>
{
    private readonly ICommentRepositoryService _commentRepositoryService;
    private readonly ICommentAttachmentStorage _attachmentStorage;
    private readonly IBlobStorageService _blobStorageService;
    private readonly AzureStorageOptions _storageOptions;

    public GetRepliesCommentsQueryHandler(
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

    public async Task<Result<List<CommentDto>>> Handle(
        GetRepliesCommentsQuery request,
        CancellationToken cancellationToken)
    {
        var response = await _commentRepositoryService.GetRepliesAsync(
            request.ParentCommentId,
            request.UserId,
            cancellationToken);

        if (response.IsFailed)
            return Result.Fail<List<CommentDto>>(response.Errors);

        var comments = response.Value
            .Select(item =>
            {
                var avatarUrl = string.IsNullOrWhiteSpace(item.AvatarBlobName)
                    ? null
                    : _blobStorageService.GetBlobUrl(
                        _storageOptions.AvatarsContainer,
                        item.AvatarBlobName
                    );

                return new CommentDto(
                    item.Comment,
                    _attachmentStorage.GetBlobUrl,
                    item.RepliesCount,
                    item.UserVote,
                    avatarUrl
                ) with
                {
                    UserName = item.UserName ?? item.Comment.UserName
                };
            })
            .ToList();

        return Result.Ok(comments);
    }
}
