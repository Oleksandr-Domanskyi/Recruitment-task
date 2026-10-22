
using Comment.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Common.DeleteComment;

public sealed class DeleteCommentCommandHandler
    : IRequestHandler<DeleteCommentCommand, Result>
{
    private readonly ICommentRepositoryService _commentRepositoryService;
    private readonly ICommentAttachmentManager _attachmentManager;

    public DeleteCommentCommandHandler(
        ICommentRepositoryService commentRepositoryService,
        ICommentAttachmentManager attachmentManager)
    {
        _commentRepositoryService = commentRepositoryService;
        _attachmentManager = attachmentManager;
    }

    public async Task<Result> Handle(
        DeleteCommentCommand request,
        CancellationToken cancellationToken)
    {
        var comment = await _commentRepositoryService.GetForUpdateAsync(request.CommentId, cancellationToken);

        if (comment is null)
            return Result.Fail("Comment not found.");

        var attachments = comment.Attachments.ToList();

        var result = await _commentRepositoryService.DeleteCommentAsync(request.CommentId, cancellationToken);

        if (result.IsFailed)
            return result;

        foreach (var attachment in attachments)
        {
            await _attachmentManager.DeleteAsync(attachment, cancellationToken);
        }

        return Result.Ok();
    }
}
