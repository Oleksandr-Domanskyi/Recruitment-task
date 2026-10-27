
using Comment.Application.DTO;
using Comment.Application.DTO.ReadModels;
using Comment.Application.Filters;
using Comment.Application.Interfaces;
using FluentResults;
using SPA.Database.Shared.UnitOfWork;

using CommentEntity = Comment.Core.Entity.Comment;

namespace Comment.Infrastructure.Services;

public sealed class CommentRepositoryService(IUnitOfWork<ICommentRepository> unitOfWork) : ICommentRepositoryService
{
    public Task<Result<PagedCommentEntities>> GetCommentsAsync(CommentFilters filters, CancellationToken cancellationToken = default)
        => unitOfWork.Repository.GetCommentsAsync(filters, cancellationToken);
    public Task<Result<CommentWithRepliesCount>> GetCommentByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => unitOfWork.Repository.GetCommentByIdAsync(id, cancellationToken);
    public Task<Result<List<UserDiscussionDto>>> GetUserDiscussionsAsync(Guid userId, int limit, CancellationToken cancellationToken = default)
        => unitOfWork.Repository.GetUserDiscussionsAsync(userId, limit, cancellationToken);
    public Task<Result<List<CommentWithRepliesCount>>> GetRepliesAsync(Guid parentCommentId, Guid? userId, CancellationToken cancellationToken = default)
        => unitOfWork.Repository.GetRepliesAsync(parentCommentId, userId, cancellationToken);

    public async Task<Result<CommentEntity>> AddCommentAsync(CommentEntity comment, CancellationToken cancellationToken = default)
    {
        var result = await unitOfWork.Repository.AddCommentAsync(comment, cancellationToken);

        if (result.IsFailed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
    }
    public async Task<CommentEntity?> GetForUpdateAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        return await unitOfWork.Repository.GetForUpdateAsync(id, cancellationToken);
    }

    public async Task<Result<CommentEntity>> UpdateCommentAsync(CommentEntity comment, CancellationToken cancellationToken = default)
    {
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(comment);
    }

    public async Task<Result<int>> UpdateCommentRatingAsync(
        Guid commentId,
        Guid userId,
        short value,
        CancellationToken cancellationToken = default)
    {
        var result = await unitOfWork.Repository.UpdateCommentRatingAsync(
            commentId,
            userId,
            value,
            cancellationToken);

        if (result.IsFailed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return result;
    }

    public async Task<Result> DeleteCommentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await unitOfWork.Repository.DeleteCommentAsync(id, cancellationToken);

        if (result.IsFailed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
    }
}
