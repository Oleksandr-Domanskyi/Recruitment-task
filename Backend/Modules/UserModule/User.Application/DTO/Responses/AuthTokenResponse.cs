using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Application.DTO.Responses
{
    public sealed record AuthTokenResponse(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt);
}