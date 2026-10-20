using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.API.Extensions;

namespace SPA.API.Extensions
{
    public static class ModulesSeedersExtension
    {
        public static async Task SeedModulesAsync(this IHost host)
        {
            await host.UserSeeders();
        }
    }
}