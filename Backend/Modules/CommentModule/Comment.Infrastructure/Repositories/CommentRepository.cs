
using Comment.Application.DTO;
using Comment.Application.DTO.ReadModels;
using Comment.Application.Filters;
using Comment.Application.Filters.Enums;
using Comment.Application.Interfaces;
using FluentResults;
using Microsoft.EntityFrameworkCore;

using CommentEntity = Comment.Core.Entity.Comment;

namespace Comment.Infrastructure.DataBase.Repositories;

public sealed class CommentRepository(CommentDbContext context) : ICommentRepository
{
    public async Task<Result<CommentWithRepliesCount>> GetCommentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var comment = await context.Comments
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new
            {
                Comment = x,
                RepliesCount = context.Comments.Count(reply => reply.ParentCommentId == x.Id)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return comment is null
            ? Result.Fail<CommentWithRepliesCount>("Comment not found.")
            : Result.Ok(new CommentWithRepliesCount(comment.Comment, comment.RepliesCount));
    }

    public async Task<Result<PagedCommentEntities>> GetCommentsAsync(CommentFilters filters, CancellationToken cancellationToken = default)
    {
        var query = context.Comments
            .AsNoTracking()
            .Where(x => x.ParentCommentId == null);

        var totalCount = await query.CountAsync(cancellationToken);

        var orderedQuery = filters.SortBy switch
        {
            CommentSortBy.CreatedAt when filters.Direction == SortDirection.Asc
                => query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),

            _ => query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
        };

        var page = Math.Max(1, filters.Page);
        var pageSize = CommentFilters.PageSize;

        var comments = await orderedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(comment => new
            {
                Comment = comment,
                RepliesCount = context.Comments.Count(reply => reply.ParentCommentId == comment.Id)
            })
            .ToListAsync(cancellationToken);

        return Result.Ok(new PagedCommentEntities(
            comments.Select(x => new CommentWithRepliesCount(x.Comment, x.RepliesCount)).ToList(),
            page,
            pageSize,
            totalCount,
            (int)Math.Ceiling((double)totalCount / pageSize)));
    }

    public async Task<Result<List<CommentEntity>>> GetCommentsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var comments = await context.Comments
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return Result.Ok(comments);
    }

    public async Task<Result<List<CommentWithRepliesCount>>> GetRepliesAsync(Guid parentCommentId, CancellationToken cancellationToken = default)
    {
        var replies = await context.Comments
            .AsNoTracking()
            .Where(x => x.ParentCommentId == parentCommentId)
            .OrderByDescending(x => x.Rating)
            .ThenByDescending(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .Select(comment => new
            {
                Comment = comment,
                RepliesCount = context.Comments.Count(reply => reply.ParentCommentId == comment.Id)
            })
            .ToListAsync(cancellationToken);

        return Result.Ok(replies
            .Select(x => new CommentWithRepliesCount(x.Comment, x.RepliesCount))
            .ToList());
    }

    public Task<CommentEntity?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Comments
            .Include(x => x.Attachments)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Result<CommentEntity>> AddCommentAsync(CommentEntity comment, CancellationToken cancellationToken = default)
    {
        await context.Comments.AddAsync(comment, cancellationToken);
        return Result.Ok(comment);
    }

    public Task<Result<CommentEntity>> UpdateCommentAsync(CommentEntity comment, CancellationToken cancellationToken = default)
    {
        context.Comments.Update(comment);
        return Task.FromResult(Result.Ok(comment));
    }

    public async Task<Result<int>> UpdateCommentRatingAsync(Guid commentId, int rating, CancellationToken cancellationToken = default)
    {
        var comment = await context.Comments.FirstOrDefaultAsync(x => x.Id == commentId, cancellationToken);

        if (comment is null)
            return Result.Fail<int>("Comment not found.");

        comment.UpdateRating(rating);
        return Result.Ok(comment.Rating);
    }

    public async Task<Result> DeleteCommentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var comment = await context.Comments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (comment is null)
            return Result.Fail("Comment not found.");

        context.Comments.Remove(comment);
        return Result.Ok();
    }
}
