
using Comment.Application.DTO;
using Microsoft.AspNetCore.Http;

namespace Comment.API.Forms;

public sealed class CreateCommentForm
{
    public string Text { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
    public IFormFile? File { get; set; }

    public Guid CaptchaId { get; set; }
    public string CaptchaAnswer { get; set; } = string.Empty;

    public CreateCommentRequest ToRequest(Stream? stream)
    {
        return new CreateCommentRequest(
            UserId: null,
            UserName: null,
            Text: Text,
            ParentCommentId: ParentCommentId,
            File: stream,
            FileName: File?.FileName,
            CaptchaId: CaptchaId,
            CaptchaAnswer: CaptchaAnswer
        );
    }
}
