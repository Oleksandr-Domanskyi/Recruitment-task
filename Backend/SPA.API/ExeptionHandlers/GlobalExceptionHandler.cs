
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SPA.API.Constants;

namespace SPA.API.ExceptionHandlers;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails problem = exception switch
        {
            ValidationException validationException =>
                new ValidationProblemDetails(
                    validationException.Errors
                        .GroupBy(x => x.PropertyName)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Select(e => e.ErrorMessage).ToArray()))
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = ApiErrors.Validation.Title,
                    Detail = ApiErrors.Validation.Detail
                },

            BadHttpRequestException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = ApiErrors.BadRequest.Title,
                Detail = ApiErrors.BadRequest.Detail
            },

            UnauthorizedAccessException => new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = ApiErrors.Forbidden.Title,
                Detail = ApiErrors.Forbidden.Detail
            },

            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = ApiErrors.InternalServerError.Title,
                Detail = ApiErrors.InternalServerError.Detail
            }
        };

        if (problem.Status >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                exception,
                ApiErrors.Logging.UnhandledException);
        }
        else
        {
            logger.LogWarning(
                exception,
                ApiErrors.Logging.RequestFailed);
        }

        context.Response.StatusCode =
            problem.Status ?? StatusCodes.Status500InternalServerError;

        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(
            problem,
            problem.GetType(),
            cancellationToken);

        return true;
    }
}
