
using Comment.Application.DTO;
using Comment.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Query.GetComments;

public class GetCommentsQueryHandle
    : IRequestHandler<GetCommentsQuery, Result<PagedCommentsDto>>
{
    private readonly ICommentRepositoryService _commentRepositoryService;
    private readonly ICommentAttachmentStorage _attachmentStorage;

    public GetCommentsQueryHandle(
        ICommentRepositoryService commentRepositoryService,
        ICommentAttachmentStorage attachmentStorage)
    {
        _commentRepositoryService = commentRepositoryService;
        _attachmentStorage = attachmentStorage;
    }

    public async Task<Result<PagedCommentsDto>> Handle(
        GetCommentsQuery request,
        CancellationToken cancellationToken)
    {
        var commentsResult = await _commentRepositoryService
            .GetCommentsAsync(request.Filter, cancellationToken);

        if (commentsResult.IsFailed)
            return Result.Fail<PagedCommentsDto>(commentsResult.Errors);

        var page = commentsResult.Value;

        var commentDtos = page.Items
            .Select(comment => new CommentDto(
                comment.Comment,
                _attachmentStorage.GetBlobUrl,
                comment.RepliesCount))
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
