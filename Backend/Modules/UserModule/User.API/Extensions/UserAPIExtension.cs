using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using User.Application.Extensions;
using User.Infrastructure.Database.Seeders;
using User.Infrastructure.Extensions;

namespace User.API.Extensions
{
    public static class UserAPIExtension
    {
        public static void AddUserModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddUserApplication();
            services.AddUserInfrastructure(configuration);
        }
        public static async Task UserSeeders(this IHost host)
        {
            await UserRoleSeeder.SeedAsync(host.Services);
        }
    }
}