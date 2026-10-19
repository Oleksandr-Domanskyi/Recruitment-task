using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.Application.DTO;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Query.GetCommentById
{
    public sealed record GetCommentByIdQuery(Guid id) : IRequest<Result<CommentDto>>;

}