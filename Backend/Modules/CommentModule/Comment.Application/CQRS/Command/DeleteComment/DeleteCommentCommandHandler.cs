using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Common.DeleteComment
{
    public class DeleteCommentCommandHandler
        : IRequestHandler<DeleteCommentCommand, Result>
    {
        private readonly ICommentRepositoryService _commentRepositoryService;

        public DeleteCommentCommandHandler(
            ICommentRepositoryService commentRepositoryService)
        {
            _commentRepositoryService = commentRepositoryService;
        }

        public async Task<Result> Handle(
            DeleteCommentCommand request,
            CancellationToken cancellationToken)
        {
            return await _commentRepositoryService.DeleteCommentAsync(
                request.CommentId,
                cancellationToken);
        }
    }
}