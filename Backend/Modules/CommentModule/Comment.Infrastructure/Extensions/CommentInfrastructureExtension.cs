using Comment.Application.Interfaces;
using Comment.Infrastructure.DataBase;
using Comment.Infrastructure.DataBase.Repositories;
using Comment.Infrastructure.Html;
using Comment.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Comment.Infrastructure.Extensions
{
    public static class CommentInfrastructureExtension
    {
        public static void AddCommentInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CommentDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ICommentRepositoryService, CommentRepositoryService>();

            services.AddScoped<ICommentHtmlSanitizer, CommentHtmlSanitizer>();
        }
    }
}