
using Comment.Application.DTO;
using Comment.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Common.UpdateComment;

public class UpdateCommentCommandHandler
    : IRequestHandler<UpdateCommentCommand, Result<CommentDto>>
{
    private readonly ICommentRepositoryService _commentRepositoryService;

    public UpdateCommentCommandHandler(
        ICommentRepositoryService commentRepositoryService)
    {
        _commentRepositoryService = commentRepositoryService;
    }

    public async Task<Result<CommentDto>> Handle(
        UpdateCommentCommand request,
        CancellationToken cancellationToken)
    {
        var commentResult = await _commentRepositoryService.GetCommentByIdAsync(
            request.ParrentId,
            cancellationToken);

        if (commentResult.IsFailed)
            return Result.Fail<CommentDto>(commentResult.Errors);

        var comment = commentResult.Value.Comment;

        comment.Update(
            request.UpdateCommentRequest.Text,
            request.UpdateCommentRequest.ImageUrl
        );

        var response = await _commentRepositoryService.UpdateCommentAsync(
            comment,
            cancellationToken);

        if (response.IsFailed)
            return Result.Fail<CommentDto>(response.Errors);

        return Result.Ok(new CommentDto(response.Value));
    }
}
