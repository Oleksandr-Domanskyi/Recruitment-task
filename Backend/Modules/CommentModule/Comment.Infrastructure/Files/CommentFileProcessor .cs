
using System.Text;
using Comment.Application.DTO;
using Comment.Application.Interfaces;
using FluentValidation;
using Shared.Constants;
using Shared.Files;

namespace Comment.Infrastructure.Files;

public sealed class CommentFileProcessor : ICommentFileProcessor
{
    public async Task<ProcessedCommentFile> ProcessAsync(Stream content, string fileName, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        var maxFileSize = extension switch
        {
            ImageFileConstants.Txt => ImageFileConstants.MaxTextFileSize,
            ImageFileConstants.Jpg or ImageFileConstants.Jpeg or
            ImageFileConstants.Png or ImageFileConstants.Gif => ImageFileConstants.MaxImageFileSize,
            _ => throw new ValidationException("Unsupported file format.")
        };

        using var buffer = await ReadFileAsync(content, maxFileSize, cancellationToken);

        return extension == ImageFileConstants.Txt
            ? ProcessTextFile(buffer, fileName)
            : ProcessImage(buffer, fileName, extension);
    }

    private static ProcessedCommentFile ProcessTextFile(MemoryStream content, string fileName)
    {
        var bytes = content.ToArray();

        try
        {
            var text = new UTF8Encoding(false, true).GetString(bytes);

            if (text.Contains('\0'))
                throw new ValidationException("TXT file contains invalid characters.");
        }
        catch (DecoderFallbackException)
        {
            throw new ValidationException("TXT file must contain valid UTF-8 text.");
        }

        return new ProcessedCommentFile(bytes, fileName, ImageFileConstants.TextContentType);
    }

    private static ProcessedCommentFile ProcessImage(MemoryStream content, string fileName, string extension)
    {
        var contentType = extension switch
        {
            ImageFileConstants.Jpg or ImageFileConstants.Jpeg => "image/jpeg",
            ImageFileConstants.Png => "image/png",
            ImageFileConstants.Gif => "image/gif",
            _ => throw new ValidationException("Unsupported image format.")
        };

        var bytes = ImageProcessor.Process(content, extension);

        return new ProcessedCommentFile(bytes, fileName, contentType);
    }

    private static async Task<MemoryStream> ReadFileAsync(Stream content, int maxFileSize, CancellationToken cancellationToken)
    {
        var bytes = new byte[maxFileSize + 1];

        var bytesRead = await content.ReadAtLeastAsync(
            bytes, maxFileSize + 1, throwOnEndOfStream: false, cancellationToken: cancellationToken);

        if (bytesRead == 0)
            throw new ValidationException("File cannot be empty.");

        if (bytesRead > maxFileSize)
            throw new ValidationException("File exceeds the allowed size.");

        return new MemoryStream(bytes, 0, bytesRead, writable: false);
    }
}
