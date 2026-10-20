using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using User.Infrastructure.Database.Seeders;

namespace User.Infrastructure.Extensions
{
    public static class SeedUserExtension
    {
        public static async Task<WebApplication> SeedUserRolesAsync(this WebApplication app)
        {
            await UserRoleSeeder.SeedAsync(app.Services);
            return app;
        }
    }
}