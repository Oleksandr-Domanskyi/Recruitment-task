
using Comment.Application.DTO;
using Comment.Application.Interfaces;
using FluentResults;
using MediatR;
using SPA.Captcha.Abstractions;
using SPA.Captcha.Events;

namespace Comment.Application.CQRS.Common.CreateComment;

public sealed class CreateCommentCommandHandler
    : IRequestHandler<CreateCommentCommand, Result<CommentDto>>
{
    private readonly ICommentRepositoryService _commentService;
    private readonly ICommentHtmlSanitizer _htmlSanitizer;
    private readonly ICommentAttachmentManager _attachmentManager;
    private readonly ICommentAttachmentStorage _attachmentStorage;
    private readonly ICaptchaService _captchaService;
    private readonly IPublisher _publisher;

    public CreateCommentCommandHandler(
        ICommentRepositoryService commentService,
        ICommentHtmlSanitizer htmlSanitizer,
        ICommentAttachmentManager attachmentManager,
        ICommentAttachmentStorage attachmentStorage,
        ICaptchaService captchaService,
        IPublisher publisher)
    {
        _commentService = commentService;
        _htmlSanitizer = htmlSanitizer;
        _attachmentManager = attachmentManager;
        _attachmentStorage = attachmentStorage;
        _captchaService = captchaService;
        _publisher = publisher;
    }

    public async Task<Result<CommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var data = request.Request;

        if (!_captchaService.Validate(data.CaptchaId, data.CaptchaAnswer))
        {
            return Result.Fail<CommentDto>(
                "Invalid or expired CAPTCHA.");
        }

        var comment = data.ToEntity(
            _htmlSanitizer.Sanitize(data.Text));

        var attachment = await _attachmentManager.UploadAsync(
            comment,
            data.File,
            data.FileName,
            cancellationToken);

        var result = await _commentService.AddCommentAsync(
            comment,
            cancellationToken);

        if (result.IsFailed)
        {
            await _attachmentManager.DeleteAsync(
                attachment,
                CancellationToken.None);

            return Result.Fail<CommentDto>(result.Errors);
        }

        await _publisher.Publish(
            new CaptchaValidatedEvent(data.CaptchaId),
            cancellationToken);

        return Result.Ok(
            new CommentDto(
                result.Value,
                _attachmentStorage.GetBlobUrl));
    }
}
