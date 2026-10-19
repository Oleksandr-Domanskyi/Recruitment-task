using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Comment.Application.Extensions
{
    public static class CommentApplicationExtension
    {
        public static void AddCommentApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CommentApplicationExtension).Assembly));
        }
    }
}