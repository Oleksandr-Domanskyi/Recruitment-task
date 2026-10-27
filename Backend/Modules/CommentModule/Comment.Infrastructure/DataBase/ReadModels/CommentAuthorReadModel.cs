using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comment.Infrastructure.DataBase.ReadModels
{
    public sealed class CommentAuthorReadModel
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
    }
}