namespace Comment.Infrastructure.Files;

internal static class CommentFileConstants
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
}