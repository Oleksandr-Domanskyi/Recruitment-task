using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comment.Application.DTO.ReadModels
{
    public sealed record CommentAttachmentDto(
        Guid Id,
        string OriginalFileName,
        string ContentType,
        string Url,
        long FileSize

    );
}