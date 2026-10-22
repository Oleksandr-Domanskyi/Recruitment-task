using MediatR;
using Microsoft.Extensions.Logging;

namespace SPA.Captcha.Events
{
    public sealed class CaptchaValidatedEventHandler : INotificationHandler<CaptchaValidatedEvent>
    {
        private readonly ILogger<CaptchaValidatedEventHandler> _logger;

        public CaptchaValidatedEventHandler(ILogger<CaptchaValidatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(
            CaptchaValidatedEvent notification,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("CAPTCHA {CaptchaId} validated successfully.", notification.CaptchaId);

            return Task.CompletedTask;
        }
    }
}