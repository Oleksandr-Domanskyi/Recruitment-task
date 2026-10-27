using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.API.Realtime;
using Comment.Application.Interfaces;

namespace SPA.API.Extensions
{
    public static class SignalRExtension
    {
        public static void AddSignalRElement(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<ICommentNotifier, SignalRCommentNotifier>();
        }
    }
}