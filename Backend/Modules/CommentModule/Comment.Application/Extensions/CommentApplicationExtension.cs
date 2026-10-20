using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.Application.Behaviors;
using Comment.Application.CQRS.Common.CreateComment;
using FluentValidation;
using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Comment.Application.Extensions
{
    public static class CommentApplicationExtension
    {
        public static void AddCommentApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CommentApplicationExtension).Assembly));

            services.AddValidatorsFromAssembly(
                typeof(CreateCommentCommandValidation).Assembly);

            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>));
        }
    }
}