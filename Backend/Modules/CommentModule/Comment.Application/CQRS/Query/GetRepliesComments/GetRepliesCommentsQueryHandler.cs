
using Comment.Application.DTO;
using Comment.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Query.GetRepliesComment;

public class GetRepliesCommentsQueryHandler
    : IRequestHandler<GetRepliesCommentsQuery, Result<List<CommentDto>>>
{
    private readonly ICommentRepositoryService _commentRepositoryService;
    private readonly ICommentAttachmentStorage _attachmentStorage;

    public GetRepliesCommentsQueryHandler(
        ICommentRepositoryService commentRepositoryService,
        ICommentAttachmentStorage attachmentStorage)
    {
        _commentRepositoryService = commentRepositoryService;
        _attachmentStorage = attachmentStorage;
    }

    public async Task<Result<List<CommentDto>>> Handle(
        GetRepliesCommentsQuery request,
        CancellationToken cancellationToken)
    {
        var response = await _commentRepositoryService.GetRepliesAsync(
            request.ParentCommentId,
            cancellationToken);

        if (response.IsFailed)
            return Result.Fail<List<CommentDto>>(response.Errors);

        var comments = response.Value
            .Select(x => new CommentDto(
                x.Comment,
                _attachmentStorage.GetBlobUrl,
                x.RepliesCount))
            .ToList();

        return Result.Ok(comments);
    }
}
