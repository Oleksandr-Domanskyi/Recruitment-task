using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SPA.API.Routes;
using SPA.Captcha.Abstractions;

namespace SPA.API.Endpoints
{
    public static class CaptchaEndpoints
    {
        public static IEndpointRouteBuilder MapCaptchaEndpoints(
            this IEndpointRouteBuilder app)
        {
            var versionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            var group = app.MapGroup(CaptchaRoutes.Base)
                .WithApiVersionSet(versionSet)
                .MapToApiVersion(new ApiVersion(1, 0))
                .WithTags(CaptchaRoutes.Tag);

            group.MapGet(CaptchaRoutes.Root, GetCaptcha);

            return app;
        }

        private static IResult GetCaptcha(
            ICaptchaService captchaService)
        {
            var captcha = captchaService.Generate();

            return Results.Ok(captcha);
        }
    }
}