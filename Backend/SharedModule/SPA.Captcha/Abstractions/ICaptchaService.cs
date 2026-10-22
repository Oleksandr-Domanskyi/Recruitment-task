using SPA.Captcha.Models;

namespace SPA.Captcha.Abstractions;

public interface ICaptchaService
{
    CaptchaResponse Generate();
    bool Validate(Guid captchaId, string answer);
}