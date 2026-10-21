using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comment.Application.DTO
{
    public sealed record ProcessedCommentFile(
        byte[] Content,
        string FileName,
        string ContentType);
}