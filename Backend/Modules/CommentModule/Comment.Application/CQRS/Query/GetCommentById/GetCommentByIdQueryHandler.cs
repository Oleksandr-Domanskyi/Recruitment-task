
using Comment.Application.DTO;
using Comment.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Query.GetCommentById;

public sealed class GetCommentByIdQueryHandler : IRequestHandler<GetCommentByIdQuery, Result<CommentDto>>
{
    private readonly ICommentRepositoryService _commentRepositoryService;
    private readonly ICommentAttachmentStorage _attachmentStorage;

    public GetCommentByIdQueryHandler(
        ICommentRepositoryService commentRepositoryService,
        ICommentAttachmentStorage attachmentStorage)
    {
        _commentRepositoryService = commentRepositoryService;
        _attachmentStorage = attachmentStorage;
    }

    public async Task<Result<CommentDto>> Handle(
        GetCommentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var commentResult = await _commentRepositoryService.GetCommentByIdAsync(
            request.id,
            cancellationToken);

        if (commentResult.IsFailed)
            return Result.Fail<CommentDto>(commentResult.Errors);

        var commentDto = new CommentDto(
            commentResult.Value.Comment,
            _attachmentStorage.GetBlobUrl,
            commentResult.Value.RepliesCount);

        return Result.Ok(commentDto);
    }
}
