
using AzureStorage.Configuration;
using Comment.Application.DTO;
using Comment.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using SPA.Shared.Abstractions.AzureStorage;


namespace Comment.Application.CQRS.Query.GetComments;

public sealed class GetCommentsQueryHandle
    : IRequestHandler<GetCommentsQuery, Result<PagedCommentsDto>>
{
    private readonly ICommentRepositoryService _commentRepositoryService;
    private readonly ICommentAttachmentStorage _attachmentStorage;
    private readonly IBlobStorageService _blobStorageService;
    private readonly AzureStorageOptions _storageOptions;

    public GetCommentsQueryHandle(
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

    public async Task<Result<PagedCommentsDto>> Handle(
        GetCommentsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _commentRepositoryService
            .GetCommentsAsync(request.Filter, cancellationToken);

        if (result.IsFailed)
            return Result.Fail<PagedCommentsDto>(result.Errors);

        var page = result.Value;

        var commentDtos = page.Items
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

        return Result.Ok(new PagedCommentsDto(
            commentDtos,
            page.Page,
            page.PageSize,
            page.TotalCount,
            page.TotalPages
        ));
    }
}
