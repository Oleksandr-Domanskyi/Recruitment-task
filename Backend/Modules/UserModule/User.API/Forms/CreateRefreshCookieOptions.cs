
using Microsoft.AspNetCore.Http;

namespace User.API.Forms;

public static class RefreshCookieOptions
{
    public static CookieOptions Create()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/v1/users",
            MaxAge = TimeSpan.FromDays(7)
        };
    }
}
