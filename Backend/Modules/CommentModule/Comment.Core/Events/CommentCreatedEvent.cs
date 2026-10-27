using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Comment.Core.Events;

public sealed record CommentCreatedEvent(
    Guid CommentId,
    Guid? ParentCommentId,
    DateTime CreatedAt
) : ICommentRealtimeEvent;
