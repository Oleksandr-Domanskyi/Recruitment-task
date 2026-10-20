
namespace SPA.API.Constants;

public static class ApiErrors
{
    public static class Validation
    {
        public const string Title = "Validation failed";
        public const string Detail =
            "One or more validation errors occurred.";
    }

    public static class BadRequest
    {
        public const string Title = "Bad request";
        public const string Detail =
            "The request is invalid.";
    }

    public static class Forbidden
    {
        public const string Title = "Forbidden";
        public const string Detail =
            "You do not have permission to perform this action.";
    }

    public static class InternalServerError
    {
        public const string Title = "Internal server error";
        public const string Detail =
            "An unexpected error occurred.";
    }
    public static class Logging
    {
        public const string UnhandledException =
            "Unhandled exception occurred.";

        public const string RequestFailed =
            "Request processing failed.";
    }
}
