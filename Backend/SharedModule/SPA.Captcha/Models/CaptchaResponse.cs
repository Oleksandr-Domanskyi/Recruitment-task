using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPA.Captcha.Models
{
    public sealed record CaptchaResponse(Guid CaptchaId, string ImageBase64);
}