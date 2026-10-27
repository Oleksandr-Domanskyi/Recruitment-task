using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.Application.DTO;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Command.CreateComment
{
    public sealed record CreateCommentCommand(CreateCommentRequest Request) : IRequest<Result<CommentDto>>;

}