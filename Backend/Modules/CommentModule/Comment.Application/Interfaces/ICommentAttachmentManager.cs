using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.Application.DTO;
using Comment.Core.Entity;

using CommentEntity = Comment.Core.Entity.Comment;

namespace Comment.Application.Interfaces
{
    public interface ICommentAttachmentManager
    {
        Task<CommentAttachment?> UploadAsync(CommentEntity comment, Stream? stream, string? fileName, CancellationToken cancellationToken);
        Task DeleteAsync(CommentAttachment? attachment, CancellationToken cancellationToken);
        Task<CommentAttachment?> UpdateAsync(CommentEntity comment, UpdateCommentRequest request, CancellationToken cancellationToken);
    }
}