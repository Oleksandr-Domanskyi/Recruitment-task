using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;
using SkiaSharp;
using SPA.Captcha.Abstractions;
using SPA.Captcha.Models;

namespace SPA.Captcha.Services;

public sealed class CaptchaService : ICaptchaService
{
    private const string Characters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int CodeLength = 6;

    private readonly IMemoryCache _cache;

    public CaptchaService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public CaptchaResponse Generate()
    {
        var code = new string(Enumerable.Range(0, CodeLength)
            .Select(_ => Characters[RandomNumberGenerator.GetInt32(Characters.Length)])
            .ToArray());

        var captchaId = Guid.NewGuid();

        _cache.Set(captchaId, code, TimeSpan.FromMinutes(5));

        using var bitmap = new SKBitmap(220, 80);
        using var canvas = new SKCanvas(bitmap);

        canvas.Clear(SKColors.White);

        using var paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true
        };

        using var font = new SKFont
        {
            Size = 38,
            Typeface = SKTypeface.FromFamilyName("Arial")
        };

        canvas.DrawText(
            code,
            20,
            53,
            SKTextAlign.Left,
            font,
            paint);

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);

        return new CaptchaResponse(
            captchaId,
            Convert.ToBase64String(data.ToArray()));
    }

    public bool Validate(Guid captchaId, string answer)
    {
        if (string.IsNullOrWhiteSpace(answer))
            return false;

        if (!_cache.TryGetValue(captchaId, out string? expectedCode))
            return false;

        _cache.Remove(captchaId);

        return string.Equals(
            expectedCode,
            answer.Trim(),
            StringComparison.OrdinalIgnoreCase);
    }
}