using MediatR;

namespace SPA.Captcha.Events
{
    public sealed class CaptchaValidatedEvent : INotification
    {
        public Guid CaptchaId { get; }

        public CaptchaValidatedEvent(Guid captchaId)
        {
            CaptchaId = captchaId;
        }
    }
}