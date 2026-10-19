
using Comment.Application.DTO;
using Comment.Application.DTO.ReadModels;
using Comment.Application.Filters;
using Comment.Application.Interfaces;
using FluentResults;

using CommentEntity = Comment.Core.Entity.Comment;

namespace Comment.Infrastructure.Services;

public class CommentRepositoryService : ICommentRepositoryService
{
    private readonly ICommentRepository _repository;

    public CommentRepositoryService(ICommentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PagedCommentEntities>> GetCommentsAsync(
        CommentFilters filters,
        CancellationToken cancellationToken = default)
        => await _repository.GetCommentsAsync(filters, cancellationToken);

    public Task<Result<CommentWithRepliesCount>> GetCommentByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => _repository.GetCommentByIdAsync(id, cancellationToken);

    public async Task<Result<List<CommentEntity>>> GetCommentsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
        => await _repository.GetCommentsByUserIdAsync(userId, cancellationToken);

    public async Task<Result<List<CommentWithRepliesCount>>> GetRepliesAsync(
        Guid parentCommentId,
        CancellationToken cancellationToken = default)
        => await _repository.GetRepliesAsync(parentCommentId, cancellationToken);

    public async Task<Result<CommentEntity>> AddCommentAsync(
        CommentEntity comment,
        CancellationToken cancellationToken = default)
        => await _repository.AddCommentAsync(comment, cancellationToken);

    public async Task<Result<CommentEntity>> UpdateCommentAsync(
        CommentEntity comment,
        CancellationToken cancellationToken = default)
        => await _repository.UpdateCommentAsync(comment, cancellationToken);

    public async Task<Result<int>> UpdateCommentRaitingAsync(
        Guid CommentId,
        int Raiting,
        CancellationToken cancellationToken)
        => await _repository.UpdateCommentRaitingAsync(CommentId, Raiting, cancellationToken);

    public async Task<Result> DeleteCommentAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => await _repository.DeleteCommentAsync(id, cancellationToken);


}
