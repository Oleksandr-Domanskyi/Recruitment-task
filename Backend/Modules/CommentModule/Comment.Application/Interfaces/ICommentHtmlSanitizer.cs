using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comment.Application.Interfaces
{
    public interface ICommentHtmlSanitizer
    {
        string Sanitize(string html);
    }
}