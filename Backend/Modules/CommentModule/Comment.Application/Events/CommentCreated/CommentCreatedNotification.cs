
using Comment.Core.Events;
using MediatR;

namespace Comment.Application.Events.CommentCreated;

public sealed record CommentCreatedNotification(
    CommentCreatedEvent Event
) : INotification;
