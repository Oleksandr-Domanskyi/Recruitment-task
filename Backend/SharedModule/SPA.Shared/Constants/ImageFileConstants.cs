using FluentValidation;

namespace Shared.Constants;

public static class ImageFileConstants
{
    public const string Txt = ".txt";
    public const string Jpg = ".jpg";
    public const string Jpeg = ".jpeg";
    public const string Png = ".png";
    public const string Gif = ".gif";

    public const string JpegFormat = "JPEG";
    public const string PngFormat = "PNG";
    public const string GifFormat = "GIF";

    public const string TextContentType = "text/plain";

    public const int MaxTextFileSize = 100 * 1024;
    public const int MaxImageFileSize = 5 * 1024 * 1024;



    public static string GetContentType(string extension) => extension switch
    {
        Jpg or Jpeg => "image/jpeg",
        Png => "image/png",
        Gif => "image/gif",
        _ => throw new ValidationException("Unsupported image format.")
    };
}