using Microsoft.Extensions.DependencyInjection;
using SPA.Captcha.Abstractions;
using SPA.Captcha.Events;
using SPA.Captcha.Services;

namespace SPA.Captcha.Extensions;

public static class CaptchaExtensions
{
    public static void AddCaptcha(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICaptchaService, CaptchaService>();

        services.AddMediatR(cfg =>
           cfg.RegisterServicesFromAssembly(
               typeof(CaptchaValidatedEventHandler).Assembly));

    }
}