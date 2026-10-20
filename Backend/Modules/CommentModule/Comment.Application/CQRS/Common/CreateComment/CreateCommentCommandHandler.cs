using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.Application.DTO;
using Comment.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Common.CreateComment
{
    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<CommentDto>>
    {
        private readonly ICommentRepositoryService _commentRepositoryService;
        private readonly ICommentHtmlSanitizer _htmlSanitizer;

        public CreateCommentCommandHandler(ICommentRepositoryService commentRepositoryService, ICommentHtmlSanitizer htmlSanitizer)
        {
            _commentRepositoryService = commentRepositoryService;
            _htmlSanitizer = htmlSanitizer;
        }
        public async Task<Result<CommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var safeText = _htmlSanitizer.Sanitize(request.Request.Text);
            var response = await _commentRepositoryService.AddCommentAsync(request.Request.ToEntity(safeText), cancellationToken);

            if (response.IsFailed)
                return Result.Fail<CommentDto>(response.Errors);
            return Result.Ok(new CommentDto(response.Value));
        }
    }
}