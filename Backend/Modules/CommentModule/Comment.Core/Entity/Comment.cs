using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comment.Core.Entity
{
    public class Comment
    {
        public Guid Id { get; private set; }

        public string UserName { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        public string? HomePage { get; private set; }
        public string Text { get; private set; } = default!;

        public int Rating { get; private set; }
        public DateTime CreatedAt { get; private set; }


        public Guid? ParentCommentId { get; private set; }

        private readonly List<Comment> _replies = new();
        public IReadOnlyCollection<Comment> Replies => _replies.AsReadOnly();

        private Comment() { }

        public Comment(
            string userName,
            string email,
            string? homePage,
            string text,
            Guid? parentCommentId = null)
        {
            Id = Guid.NewGuid();
            UserName = userName;
            Email = email;
            HomePage = homePage;
            Text = text;
            ParentCommentId = parentCommentId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}