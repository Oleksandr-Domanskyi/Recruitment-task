using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.Core.Events;
using MediatR;

namespace Comment.Application.Events.CommentRatingChanged
{
    public sealed record CommentRatingChangedNotification(
        CommentRatingChangedEvent Event
    ) : INotification;
}