using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPA.API.ExceptionHandlers;

namespace SPA.API.Extensions
{
    public static class AddExceptionHandlers
    {
        public static void AddExeptionHandlers(this IServiceCollection services)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
        }
    }
}