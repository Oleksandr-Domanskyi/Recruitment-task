
namespace Comment.Core.Entity;

public class Comment
{
    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; }
    public string? UserName { get; private set; }
    public string Text { get; private set; } = default!;
    public string? ImageUrl { get; private set; }
    public int Rating { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? ParentCommentId { get; private set; }

    private readonly List<Comment> _replies = new();
    public IReadOnlyCollection<Comment> Replies => _replies.AsReadOnly();

    private readonly List<CommentAttachment> _attachments = new();
    public IReadOnlyCollection<CommentAttachment> Attachments => _attachments.AsReadOnly();

    private Comment() { }

    public Comment(
        string text,
        Guid? userId = null,
        string? userName = null,
        Guid? parentCommentId = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        UserName = userName;
        Text = text;
        ParentCommentId = parentCommentId;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string text)
    {
        Text = text;
    }

    public void AddAttachment(CommentAttachment attachment)
    {
        ArgumentNullException.ThrowIfNull(attachment);

        if (attachment.CommentId != Id)
            throw new ArgumentException(
                "Attachment belongs to another comment.",
                nameof(attachment));

        _attachments.Add(attachment);
    }

    public void RemoveAttachment(CommentAttachment attachment)
    {
        _attachments.Remove(attachment);
    }


    public void UpdateRating(int difference)
    {
        Rating += difference;
    }

}
