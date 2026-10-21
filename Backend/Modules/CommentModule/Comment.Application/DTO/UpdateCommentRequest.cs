using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comment.Application.DTO
{
    public sealed record UpdateCommentRequest(
        string Text,
        Stream? File,
        string? FileName,
        bool RemoveAttachment
    );
}