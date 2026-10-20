using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Comment.Application.Extensions;
using Comment.Infrastructure.Extensions;


namespace Comment.API.Extensions
{
    public static class CommentAPIExtension
    {
        public static void AddCommentModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCommentApplication();
            services.AddCommentInfrastructure(configuration);
        }
    }
}