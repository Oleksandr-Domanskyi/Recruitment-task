using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.Application.DTO;
using Microsoft.AspNetCore.Http;

namespace Comment.API.Forms
{
    public sealed class CreateCommentForm
    {
        public Guid? UserId { get; set; }
        public string Text { get; set; } = string.Empty;
        public Guid? ParentCommentId { get; set; }
        public IFormFile? File { get; set; }

        public CreateCommentRequest ToRequest(Stream? stream)
        {
            return new CreateCommentRequest(
                UserId,
                Text,
                ParentCommentId,
                stream,
                File?.FileName);
        }
    }


}