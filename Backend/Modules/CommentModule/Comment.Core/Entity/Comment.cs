
namespace Comment.Core.Entity;

public class Comment
{
    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; }
    public string Text { get; private set; } = default!;
    public string? ImageUrl { get; private set; }
    public int Rating { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? ParentCommentId { get; private set; }
    private readonly List<Comment> _replies = new();
    public IReadOnlyCollection<Comment> Replies => _replies.AsReadOnly();

    private Comment() { }

    public Comment(
        string text,
        Guid? userId = null,
        Guid? parentCommentId = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Text = text;
        ParentCommentId = parentCommentId;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string text, string? imageUrl)
    {
        Text = text;
        ImageUrl = imageUrl;
    }

    public void UpdateRaiting(int rating) => Rating = rating < 0 ? Rating - 1 : Rating + 1;
}
