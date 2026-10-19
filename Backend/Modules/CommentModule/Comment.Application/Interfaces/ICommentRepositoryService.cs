
using Comment.Application.DTO;
using Comment.Application.DTO.ReadModels;
using Comment.Application.Filters;
using FluentResults;

using CommentEntity = Comment.Core.Entity.Comment;

namespace Comment.Application.Interfaces;

public interface ICommentRepositoryService
{
    Task<Result<PagedCommentEntities>> GetCommentsAsync(
        CommentFilters filters,
        CancellationToken cancellationToken = default);

    Task<Result<CommentWithRepliesCount>> GetCommentByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<List<CommentEntity>>> GetCommentsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Result<List<CommentWithRepliesCount>>> GetRepliesAsync(
        Guid parentCommentId,
        CancellationToken cancellationToken = default);

    Task<Result<CommentEntity>> AddCommentAsync(
        CommentEntity comment,
        CancellationToken cancellationToken = default);

    Task<Result<CommentEntity>> UpdateCommentAsync(
        CommentEntity comment,
        CancellationToken cancellationToken = default);

    Task<Result<int>> UpdateCommentRaitingAsync(
        Guid CommentId,
        int Raiting,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteCommentAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
