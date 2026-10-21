using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Comment.API.Extensions;
using User.API.Extensions;
using AzureStorage.Extensions;

namespace SPA.API.Extensions
{
    public static class ModulesExtension
    {
        public static void AddModules(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCommentModule(configuration);
            services.AddUserModule(configuration);


            //Integration
            //services.AddAzureStorage(configuration);
        }
    }
}