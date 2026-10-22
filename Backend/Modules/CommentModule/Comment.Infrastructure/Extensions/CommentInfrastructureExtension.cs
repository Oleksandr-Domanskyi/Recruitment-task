using Comment.Application.Interfaces;
using Comment.Infrastructure.DataBase;
using Comment.Infrastructure.DataBase.Repositories;
using Comment.Infrastructure.Files;
using Comment.Infrastructure.Html;
using Comment.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SPA.Database.Shared.Extensions;
using SPA.Database.Shared.UnitOfWork;

namespace Comment.Infrastructure.Extensions;

public static class CommentInfrastructureExtension
{
    public static void AddCommentInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseContext<CommentDbContext>(configuration);

        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ICommentRepositoryService, CommentRepositoryService>();
        services.AddScoped<ICommentHtmlSanitizer, CommentHtmlSanitizer>();
        services.AddScoped<ICommentFileProcessor, CommentFileProcessor>();
        services.AddScoped<ICommentAttachmentStorage, CommentAttachmentStorage>();

        services.AddScoped<IUnitOfWork<ICommentRepository>, UnitOfWork<CommentDbContext, ICommentRepository>>();
    }
}