using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comment.API.Routes
{
    public static class CommentRoutes
    {
        public const string Tag = "Comments";
        public const string Base = "/api/v{version:apiVersion}/comments";
        public const string Root = "/";
        public const string ById = "/{id:guid}";
        public const string Replies = "/{parentCommentId:guid}/replies";
        public const string Rating = "/{id:guid}/rating";
    }
}