using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comment.Application.DTO.ReadModels
{
    public sealed record UserDiscussionDto(
        Guid RootCommentId,
        string Preview,
        DateTime LastActivityAt
    );
}