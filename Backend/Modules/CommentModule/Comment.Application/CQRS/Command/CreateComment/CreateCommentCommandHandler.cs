
using Comment.Application.DTO;
using Comment.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Common.CreateComment;

public sealed class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<CommentDto>>
{
    private readonly ICommentRepositoryService _commentService;
    private readonly ICommentHtmlSanitizer _htmlSanitizer;
    private readonly ICommentAttachmentManager _attachmentManager;

    public CreateCommentCommandHandler(ICommentRepositoryService commentService, ICommentHtmlSanitizer htmlSanitizer, ICommentAttachmentManager attachmentManager)
    {
        _commentService = commentService;
        _htmlSanitizer = htmlSanitizer;
        _attachmentManager = attachmentManager;
    }

    public async Task<Result<CommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var data = request.Request;
        var comment = data.ToEntity(_htmlSanitizer.Sanitize(data.Text));

        var attachment = await _attachmentManager.UploadAsync(comment, data.File, data.FileName, cancellationToken);

        try
        {
            var result = await _commentService.AddCommentAsync(comment, cancellationToken);

            if (result.IsFailed)
            {
                await _attachmentManager.DeleteAsync(attachment, CancellationToken.None);
                return Result.Fail<CommentDto>(result.Errors);
            }

            return Result.Ok(new CommentDto(result.Value));
        }
        catch
        {
            await _attachmentManager.DeleteAsync(attachment, CancellationToken.None);
            throw;
        }
    }
}
