
using Comment.Application.Filters;
using Comment.Application.DTO;
using FluentResults;

using CommentEntity = Comment.Core.Entity.Comment;
using Comment.Application.DTO.ReadModels;
using Comment.Core.Entity;

namespace Comment.Application.Interfaces;

public interface ICommentRepository
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

    Task<CommentEntity?> GetForUpdateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<CommentEntity>> AddCommentAsync(
        CommentEntity comment,
        CancellationToken cancellationToken = default);


    Task<Result<CommentEntity>> UpdateCommentAsync(
        CommentEntity comment,
        CancellationToken cancellationToken = default);

    Task<Result<int>> UpdateCommentRatingAsync(
      Guid commentId,
      int rating,
      CancellationToken cancellationToken = default);

    Task<Result> DeleteCommentAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
