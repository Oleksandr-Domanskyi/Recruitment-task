
using FluentValidation;
using Shared.Constants;
using SkiaSharp;

namespace Shared.Files;

public static class ImageProcessor
{
    private const int MaxWidth = 320;
    private const int MaxHeight = 240;

    public static byte[] Process(Stream content, string extension)
    {
        using var codec = SKCodec.Create(content);

        if (codec is null)
            throw new ValidationException("Invalid image format.");

        var expectedFormat = extension switch
        {
            ImageFileConstants.Jpg or ImageFileConstants.Jpeg => SKEncodedImageFormat.Jpeg,
            ImageFileConstants.Png => SKEncodedImageFormat.Png,
            ImageFileConstants.Gif => SKEncodedImageFormat.Gif,
            _ => throw new ValidationException("Unsupported image format.")
        };

        if (codec.EncodedFormat != expectedFormat)
            throw new ValidationException("File content does not match its extension.");

        using var bitmap = SKBitmap.Decode(codec);

        if (bitmap is null)
            throw new ValidationException("Image is corrupted.");

        if (bitmap.Width <= MaxWidth && bitmap.Height <= MaxHeight)
        {
            content.Position = 0;

            using var output = new MemoryStream();
            content.CopyTo(output);

            return output.ToArray();
        }

        if (expectedFormat == SKEncodedImageFormat.Gif)
            throw new ValidationException("GIF exceeds the allowed dimensions.");

        var scale = Math.Min((double)MaxWidth / bitmap.Width, (double)MaxHeight / bitmap.Height);
        var width = Math.Max(1, (int)Math.Round(bitmap.Width * scale));
        var height = Math.Max(1, (int)Math.Round(bitmap.Height * scale));

        using var resized = bitmap.Resize(new SKImageInfo(width, height), SKSamplingOptions.Default);

        if (resized is null)
            throw new ValidationException("Image could not be resized.");

        using var image = SKImage.FromBitmap(resized);
        using var encoded = image.Encode(expectedFormat, 90);

        if (encoded is null)
            throw new ValidationException("Image could not be encoded.");

        return encoded.ToArray();
    }
}
