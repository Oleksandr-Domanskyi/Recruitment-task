using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comment.Core.Entity
{
    public sealed class CommentVote
    {
        public Guid CommentId { get; set; }
        public Guid UserId { get; set; }
        public short Value { get; set; }
    }
}