using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Comment.Core.Events;

public sealed record CommentRatingChangedEvent(
    Guid CommentId,
    int Rating
) : ICommentRealtimeEvent;