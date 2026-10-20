
namespace User.API.Routes;

public static class UserRoutes
{
    public const string Base = "/api/v{version:apiVersion}/users";

    public const string Register = "/register";
    public const string Login = "/login";
    public const string Refresh = "/refresh";
    public const string Logout = "/logout";

    public const string Me = "/me";
}
