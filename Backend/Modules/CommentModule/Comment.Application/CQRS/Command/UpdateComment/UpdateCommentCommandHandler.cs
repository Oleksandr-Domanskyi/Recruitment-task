
using Comment.Application.DTO;
using Comment.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Command.UpdateComment;

public sealed class UpdateCommentCommandHandler
    : IRequestHandler<UpdateCommentCommand, Result<CommentDto>>
{
    private readonly ICommentRepositoryService _commentRepositoryService;
    private readonly ICommentHtmlSanitizer _htmlSanitizer;
    private readonly ICommentAttachmentManager _attachmentManager;
    private readonly ICommentAttachmentStorage _attachmentStorage;

    public UpdateCommentCommandHandler(
        ICommentRepositoryService commentRepositoryService,
        ICommentHtmlSanitizer htmlSanitizer,
        ICommentAttachmentManager attachmentManager,
        ICommentAttachmentStorage attachmentStorage)
    {
        _commentRepositoryService = commentRepositoryService;
        _htmlSanitizer = htmlSanitizer;
        _attachmentManager = attachmentManager;
        _attachmentStorage = attachmentStorage;
    }

    public async Task<Result<CommentDto>> Handle(
        UpdateCommentCommand request,
        CancellationToken cancellationToken)
    {
        var comment = await _commentRepositoryService.GetForUpdateAsync(
            request.CommentId,
            cancellationToken);

        if (comment is null)
            return Result.Fail<CommentDto>("Comment not found.");

        comment.Update(
            _htmlSanitizer.Sanitize(request.UpdateCommentRequest.Text));

        var oldAttachment = await _attachmentManager.UpdateAsync(
            comment,
            request.UpdateCommentRequest,
            cancellationToken);

        var response = await _commentRepositoryService.UpdateCommentAsync(
            comment,
            cancellationToken);

        if (response.IsFailed)
            return Result.Fail<CommentDto>(response.Errors);

        await _attachmentManager.DeleteAsync(
            oldAttachment,
            cancellationToken);

        return Result.Ok(new CommentDto(
            response.Value,
            _attachmentStorage.GetBlobUrl));
    }
}
