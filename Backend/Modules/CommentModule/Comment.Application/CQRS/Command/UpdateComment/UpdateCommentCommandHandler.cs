
using Comment.Application.DTO;
using Comment.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Common.UpdateComment;

public sealed class UpdateCommentCommandHandler
    : IRequestHandler<UpdateCommentCommand, Result<CommentDto>>
{
    private readonly ICommentRepositoryService _commentRepositoryService;
    private readonly ICommentHtmlSanitizer _htmlSanitizer;
    private readonly ICommentAttachmentManager _attachmentManager;

    public UpdateCommentCommandHandler(
        ICommentRepositoryService commentRepositoryService,
        ICommentHtmlSanitizer htmlSanitizer,
        ICommentAttachmentManager attachmentManager)
    {
        _commentRepositoryService = commentRepositoryService;
        _htmlSanitizer = htmlSanitizer;
        _attachmentManager = attachmentManager;
    }

    public async Task<Result<CommentDto>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var result = await _commentRepositoryService.GetCommentByIdAsync(request.CommentId, cancellationToken);

        if (result.IsFailed)
            return Result.Fail<CommentDto>(result.Errors);

        var comment = result.Value.Comment;
        comment.Update(_htmlSanitizer.Sanitize(request.UpdateCommentRequest.Text));

        var oldAttachment = await _attachmentManager.UpdateAsync(comment, request.UpdateCommentRequest, cancellationToken);
        var response = await _commentRepositoryService.UpdateCommentAsync(comment, cancellationToken);

        if (response.IsFailed)
            return Result.Fail<CommentDto>(response.Errors);

        await _attachmentManager.DeleteAsync(oldAttachment, cancellationToken);

        return Result.Ok(new CommentDto(response.Value));
    }
}
