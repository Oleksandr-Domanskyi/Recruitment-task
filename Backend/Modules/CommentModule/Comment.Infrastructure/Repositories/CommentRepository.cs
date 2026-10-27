
using Comment.Application.DTO;
using Comment.Application.DTO.ReadModels;
using Comment.Application.Filters;
using Comment.Application.Filters.Enums;
using Comment.Application.Interfaces;
using Comment.Core.Entity;
using FluentResults;
using Microsoft.EntityFrameworkCore;

using CommentEntity = Comment.Core.Entity.Comment;

namespace Comment.Infrastructure.DataBase.Repositories;

public sealed class CommentRepository(CommentDbContext context) : ICommentRepository
{

    public async Task<Result<CommentWithRepliesCount>> GetCommentByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var comment = await context.Comments
            .AsNoTracking()
            .Include(x => x.Attachments)
            .Where(x => x.Id == id)
            .GroupJoin(
                context.CommentAuthors.AsNoTracking(),
                comment => comment.UserId,
                user => (Guid?)user.Id,
                (comment, users) => new { comment, users })
            .SelectMany(
                x => x.users.DefaultIfEmpty(),
                (x, user) => new
                {
                    Comment = x.comment,
                    UserName = user != null ? user.UserName : null,
                    AvatarBlobName = user != null ? user.AvatarUrl : null
                })
            .Select(x => new
            {
                x.Comment,
                x.UserName,
                x.AvatarBlobName,

                RepliesCount = context.Comments.Count(
                    reply => reply.ParentCommentId == x.Comment.Id)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (comment is null)
        {
            return Result.Fail<CommentWithRepliesCount>(
                "Comment not found.");
        }

        return Result.Ok(new CommentWithRepliesCount(
            comment.Comment,
            comment.RepliesCount,
            UserName: comment.UserName,
            AvatarBlobName: comment.AvatarBlobName
        ));
    }



    public async Task<Result<PagedCommentEntities>> GetCommentsAsync(
        CommentFilters filters,
        CancellationToken cancellationToken = default)
    {
        var query = context.Comments
            .AsNoTracking()
            .Include(x => x.Attachments)
            .Where(x => x.ParentCommentId == null)
            .GroupJoin(
                context.CommentAuthors.AsNoTracking(),
                comment => comment.UserId,
                user => (Guid?)user.Id,
                (comment, users) => new { comment, users })
            .SelectMany(
                x => x.users.DefaultIfEmpty(),
                (x, user) => new
                {
                    Comment = x.comment,
                    UserName = user != null ? user.UserName : null,
                    Email = user != null ? user.Email : null,
                    AvatarBlobName = user != null ? user.AvatarUrl : null
                });

        if (!string.IsNullOrWhiteSpace(filters.Username))
        {
            query = query.Where(x =>
                x.UserName != null &&
                x.UserName.Contains(filters.Username));
        }

        if (!string.IsNullOrWhiteSpace(filters.Email))
        {
            query = query.Where(x =>
                x.Email != null &&
                x.Email.Contains(filters.Email));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var orderedQuery = filters.SortBy switch
        {
            CommentSortBy.Rating when filters.Direction == SortDirection.Asc
                => query.OrderBy(x => x.Comment.Rating)
                    .ThenByDescending(x => x.Comment.CreatedAt)
                    .ThenBy(x => x.Comment.Id),

            CommentSortBy.Rating
                => query.OrderByDescending(x => x.Comment.Rating)
                    .ThenByDescending(x => x.Comment.CreatedAt)
                    .ThenBy(x => x.Comment.Id),

            CommentSortBy.CreatedAt when filters.Direction == SortDirection.Asc
                => query.OrderBy(x => x.Comment.CreatedAt)
                    .ThenBy(x => x.Comment.Id),

            CommentSortBy.UserName when filters.Direction == SortDirection.Asc
                => query.OrderBy(x => x.UserName == null)
                    .ThenBy(x => x.UserName)
                    .ThenBy(x => x.Comment.Id),

            CommentSortBy.UserName
                => query.OrderBy(x => x.UserName == null)
                    .ThenByDescending(x => x.UserName)
                    .ThenByDescending(x => x.Comment.Id),

            CommentSortBy.Email when filters.Direction == SortDirection.Asc
                => query.OrderBy(x => x.Email == null)
                    .ThenBy(x => x.Email)
                    .ThenBy(x => x.Comment.Id),

            CommentSortBy.Email
                => query.OrderBy(x => x.Email == null)
                    .ThenByDescending(x => x.Email)
                    .ThenByDescending(x => x.Comment.Id),

            _ => query.OrderByDescending(x => x.Comment.CreatedAt)
                .ThenByDescending(x => x.Comment.Id)
        };

        var page = Math.Max(1, filters.Page);
        var pageSize = CommentFilters.PageSize;

        var comments = await orderedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                x.Comment,
                x.UserName,
                x.AvatarBlobName,
                RepliesCount = context.Comments.Count(
                    reply => reply.ParentCommentId == x.Comment.Id)
            })
            .ToListAsync(cancellationToken);

        return Result.Ok(new PagedCommentEntities(
            comments.Select(x => new CommentWithRepliesCount(
                x.Comment,
                x.RepliesCount,
                UserName: x.UserName,
                AvatarBlobName: x.AvatarBlobName
            )).ToList(),
            page,
            pageSize,
            totalCount,
            (int)Math.Ceiling((double)totalCount / pageSize)
        ));
    }


    public async Task<Result<List<CommentWithRepliesCount>>> GetRepliesAsync(
        Guid parentCommentId,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        var replies = await context.Comments
            .AsNoTracking()
            .Include(x => x.Attachments)
            .Where(x => x.ParentCommentId == parentCommentId)
            .GroupJoin(
                context.CommentAuthors.AsNoTracking(),
                comment => comment.UserId,
                user => (Guid?)user.Id,
                (comment, users) => new { comment, users })
            .SelectMany(
                x => x.users.DefaultIfEmpty(),
                (x, user) => new
                {
                    Comment = x.comment,
                    UserName = user != null ? user.UserName : null,
                    AvatarBlobName = user != null ? user.AvatarUrl : null
                })
            .OrderByDescending(x => x.Comment.Rating)
            .ThenByDescending(x => x.Comment.CreatedAt)
            .ThenBy(x => x.Comment.Id)
            .Select(x => new
            {
                x.Comment,
                x.UserName,
                x.AvatarBlobName,

                RepliesCount = context.Comments.Count(
                    reply => reply.ParentCommentId == x.Comment.Id),

                UserVote = userId.HasValue
                    ? context.commentVotes
                        .Where(vote =>
                            vote.CommentId == x.Comment.Id &&
                            vote.UserId == userId.Value)
                        .Select(vote => (short?)vote.Value)
                        .FirstOrDefault() ?? (short)0
                    : (short)0
            })
            .ToListAsync(cancellationToken);

        return Result.Ok(
            replies
                .Select(x => new CommentWithRepliesCount(
                    x.Comment,
                    x.RepliesCount,
                    x.UserVote,
                    x.UserName,
                    x.AvatarBlobName
                ))
                .ToList()
        );
    }



    public async Task<Result<List<UserDiscussionDto>>> GetUserDiscussionsAsync(
        Guid userId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        limit = Math.Clamp(limit, 1, 20);

        var userComments = await context.Comments
            .AsNoTracking()
            .Where(x => x.UserId == userId)

            .Select(x => new
            {
                x.Id,
                x.ParentCommentId,
                x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        if (userComments.Count == 0)
            return Result.Ok(new List<UserDiscussionDto>());


        var parentById = userComments.ToDictionary(
            x => x.Id,
            x => x.ParentCommentId
        );

        var pendingIds = userComments
            .Where(x => x.ParentCommentId.HasValue)
            .Select(x => x.ParentCommentId!.Value)
            .Where(id => !parentById.ContainsKey(id))
            .ToHashSet();


        while (pendingIds.Count > 0)
        {
            var parents = await context.Comments
                .AsNoTracking()
                .Where(x => pendingIds.Contains(x.Id))
                .Select(x => new
                {
                    x.Id,
                    x.ParentCommentId
                })
                .ToListAsync(cancellationToken);

            var nextIds = new HashSet<Guid>();

            foreach (var parent in parents)
            {
                if (!parentById.TryAdd(
                    parent.Id,
                    parent.ParentCommentId))
                {
                    continue;
                }

                if (parent.ParentCommentId is Guid nextParentId &&
                    !parentById.ContainsKey(nextParentId))
                {
                    nextIds.Add(nextParentId);
                }
            }

            nextIds.RemoveWhere(parentById.ContainsKey);
            pendingIds = nextIds;
        }

        static Guid? FindRoot(
            Guid commentId,
            IReadOnlyDictionary<Guid, Guid?> parentMap)
        {
            var visited = new HashSet<Guid>();

            while (visited.Add(commentId) &&
                   parentMap.TryGetValue(commentId, out var parentId))
            {
                if (!parentId.HasValue)
                    return commentId;

                commentId = parentId.Value;
            }

            return null;
        }


        var latestActivityByRoot = new Dictionary<Guid, DateTime>();
        foreach (var comment in userComments)
        {
            var rootId = FindRoot(comment.Id, parentById);

            if (!rootId.HasValue)
                continue;

            if (!latestActivityByRoot.TryGetValue(
                    rootId.Value,
                    out var previousDate) ||
                comment.CreatedAt > previousDate)
            {
                latestActivityByRoot[rootId.Value] = comment.CreatedAt;
            }
        }

        var recentRoots = latestActivityByRoot
            .OrderByDescending(x => x.Value)
            .ThenBy(x => x.Key)
            .Take(limit)
            .ToList();

        if (recentRoots.Count == 0)
            return Result.Ok(new List<UserDiscussionDto>());

        var rootIds = recentRoots
            .Select(x => x.Key)
            .ToArray();

        var rootTexts = await context.Comments
            .AsNoTracking()
            .Where(x => rootIds.Contains(x.Id))
            .Select(x => new
            {
                x.Id,
                x.Text
            })
            .ToDictionaryAsync(
                x => x.Id,
                x => x.Text,
                cancellationToken
            );

        var discussions = recentRoots
            .Where(x => rootTexts.ContainsKey(x.Key))
            .Select(x => new UserDiscussionDto(
                x.Key,
                rootTexts[x.Key],
                x.Value
            ))
            .ToList();

        return Result.Ok(discussions);
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

    public async Task<Result<int>> UpdateCommentRatingAsync(
        Guid commentId,
        Guid userId,
        short value,
        CancellationToken cancellationToken = default)
    {
        if (value is not (-1 or 0 or 1))
            return Result.Fail<int>("Invalid vote value.");

        var comment = await context.Comments
            .FirstOrDefaultAsync(
                x => x.Id == commentId,
                cancellationToken);

        if (comment is null)
            return Result.Fail<int>("Comment not found.");

        var existingVote = await context.commentVotes
            .FirstOrDefaultAsync(
                x => x.CommentId == commentId && x.UserId == userId,
                cancellationToken);

        var previousValue = existingVote?.Value ?? 0;

        if (previousValue == value)
            return Result.Ok(comment.Rating);

        var difference = value - previousValue;

        if (existingVote is null)
        {
            if (value != 0)
            {
                await context.commentVotes.AddAsync(
                    new CommentVote
                    {
                        CommentId = commentId,
                        UserId = userId,
                        Value = value,
                    },
                    cancellationToken);
            }
        }
        else if (value == 0)
        {
            context.commentVotes.Remove(existingVote);
        }
        else
        {
            existingVote.Value = value;
        }
        comment.UpdateRating(difference);
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
