using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.Application.DTO;

namespace Comment.Application.Interfaces
{
    public interface ICommentFileProcessor
    {
        Task<ProcessedCommentFile> ProcessAsync(
            Stream content,
            string fileName,
            CancellationToken cancellationToken = default);
    }
}