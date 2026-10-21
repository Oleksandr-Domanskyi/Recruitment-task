using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.Application.DTO;
using Microsoft.AspNetCore.Http;

namespace Comment.API.Forms
{
    public sealed class UpdateCommentForm
    {
        public string Text { get; set; } = string.Empty;
        public IFormFile? File { get; set; }
        public bool RemoveAttachment { get; set; }

        public UpdateCommentRequest ToRequest(Stream? stream)
        {
            return new UpdateCommentRequest(
                Text,
                stream,
                File?.FileName,
                RemoveAttachment);
        }
    }
}