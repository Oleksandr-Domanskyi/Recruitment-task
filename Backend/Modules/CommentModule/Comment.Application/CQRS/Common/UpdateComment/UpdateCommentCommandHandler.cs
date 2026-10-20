
using Comment.Application.DTO;
using Comment.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Common.UpdateComment;

public class UpdateCommentCommandHandler(
    ICommentRepositoryService commentRepositoryService,
    ICommentHtmlSanitizer htmlSanitizer)
    : IRequestHandler<UpdateCommentCommand, Result<CommentDto>>
{
    public async Task<Result<CommentDto>> Handle(
        UpdateCommentCommand request,
        CancellationToken cancellationToken)
    {
        var commentResult = await commentRepositoryService.GetCommentByIdAsync(
            request.ParrentId,
            cancellationToken);

        if (commentResult.IsFailed)
            return Result.Fail<CommentDto>(commentResult.Errors);

        var comment = commentResult.Value.Comment;

        var safeText = htmlSanitizer.Sanitize(
            request.UpdateCommentRequest.Text);

        comment.Update(
            safeText,
            request.UpdateCommentRequest.ImageUrl
        );

        var response = await commentRepositoryService.UpdateCommentAsync(
            comment,
            cancellationToken);

        if (response.IsFailed)
            return Result.Fail<CommentDto>(response.Errors);

        return Result.Ok(new CommentDto(response.Value));
    }
}
