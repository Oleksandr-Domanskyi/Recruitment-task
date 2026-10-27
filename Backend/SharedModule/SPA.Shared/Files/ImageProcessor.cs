
using FluentValidation;
using ImageMagick;
using Shared.Constants;
using SkiaSharp;

namespace Shared.Files;

public static class ImageProcessor
{
    private const int MaxWidth = 320;
    private const int MaxHeight = 240;

    public static byte[] Process(Stream content, string extension)
    {
        content.Position = 0;

        using var codec = SKCodec.Create(content);

        if (codec is null)
            throw new ValidationException("Invalid image format.");

        var expectedFormat = extension switch
        {
            ImageFileConstants.Jpg or ImageFileConstants.Jpeg
                => SKEncodedImageFormat.Jpeg,

            ImageFileConstants.Png
                => SKEncodedImageFormat.Png,

            ImageFileConstants.Gif
                => SKEncodedImageFormat.Gif,

            _ => throw new ValidationException("Unsupported image format.")
        };

        if (codec.EncodedFormat != expectedFormat)
            throw new ValidationException(
                "File content does not match its extension.");

        var originalWidth = codec.Info.Width;
        var originalHeight = codec.Info.Height;

        if (originalWidth <= 0 || originalHeight <= 0)
            throw new ValidationException("Invalid image dimensions.");

        if (expectedFormat == SKEncodedImageFormat.Gif)
        {
            return ProcessGif(
                content,
                originalWidth,
                originalHeight
            );
        }

        using var bitmap = SKBitmap.Decode(codec);

        if (bitmap is null)
            throw new ValidationException("Image is corrupted.");

        if (bitmap.Width <= MaxWidth &&
            bitmap.Height <= MaxHeight)
        {
            return ReadOriginal(content);
        }

        var (width, height) = CalculateDimensions(
            bitmap.Width,
            bitmap.Height
        );

        using var resized = bitmap.Resize(
            new SKImageInfo(width, height),
            SKSamplingOptions.Default
        );

        if (resized is null)
            throw new ValidationException("Image could not be resized.");

        using var image = SKImage.FromBitmap(resized);
        using var encoded = image.Encode(expectedFormat, 90);

        if (encoded is null)
            throw new ValidationException("Image could not be encoded.");

        return encoded.ToArray();
    }

    private static byte[] ProcessGif(
        Stream content,
        int originalWidth,
        int originalHeight)
    {
        content.Position = 0;

        if (originalWidth <= MaxWidth &&
            originalHeight <= MaxHeight)
        {
            return ReadOriginal(content);
        }

        var (width, height) = CalculateDimensions(
            originalWidth,
            originalHeight
        );

        content.Position = 0;

        try
        {
            using var frames = new MagickImageCollection(content);

            if (frames.Count == 0)
                throw new ValidationException(
                    "GIF contains no valid frames.");

            frames.Coalesce();

            foreach (var frame in frames)
            {
                frame.Resize(
                    (uint)width,
                    (uint)height
                );
            }

            frames.Optimize();

            using var output = new MemoryStream();

            frames.Write(output, MagickFormat.Gif);

            return output.ToArray();
        }
        catch (MagickException)
        {
            throw new ValidationException(
                "GIF is corrupted or could not be processed.");
        }
    }

    private static (int Width, int Height) CalculateDimensions(
        int width,
        int height)
    {
        var scale = Math.Min(
            (double)MaxWidth / width,
            (double)MaxHeight / height
        );

        scale = Math.Min(scale, 1.0);

        var newWidth = Math.Max(
            1,
            (int)Math.Floor(width * scale)
        );

        var newHeight = Math.Max(
            1,
            (int)Math.Floor(height * scale)
        );

        return (newWidth, newHeight);
    }

    private static byte[] ReadOriginal(Stream content)
    {
        content.Position = 0;

        using var output = new MemoryStream();

        content.CopyTo(output);

        return output.ToArray();
    }
}
