using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SPA.Database.Shared.Extensions
{
    public static class SPADatabaseExtension
    {
        public static void AddDatabaseContext<T>(this IServiceCollection services, IConfiguration configuration)
        where T : DbContext
        {
            services.AddDbContext<T>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    e => e.MigrationsAssembly(typeof(T).Assembly.FullName));
            });
        }
    }
}