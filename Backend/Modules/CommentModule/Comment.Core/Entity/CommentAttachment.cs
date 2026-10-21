using Comment.Core.Entity.Enums;

namespace Comment.Core.Entity;

public sealed class CommentAttachment
{
    public Guid Id { get; private set; }
    public Guid CommentId { get; private set; }
    public string BlobName { get; private set; } = null!;
    public string OriginalFileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long FileSize { get; private set; }
    public AttachmentType Type { get; private set; }

    public Comment Comment { get; private set; } = null!;

    private CommentAttachment() { }

    public CommentAttachment(
        Guid commentId,
        string blobName,
        string originalFileName,
        string contentType,
        long fileSize,
        AttachmentType type)
    {
        Id = Guid.NewGuid();
        CommentId = commentId;
        BlobName = blobName;
        OriginalFileName = originalFileName;
        ContentType = contentType;
        FileSize = fileSize;
        Type = type;
    }
}