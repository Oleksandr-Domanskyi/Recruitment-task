using Comment.Application.DTO;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Query.GetRepliesComment;

public sealed record GetRepliesCommentsQuery(
    Guid ParentCommentId,
    Guid? UserId
) : IRequest<Result<List<CommentDto>>>;