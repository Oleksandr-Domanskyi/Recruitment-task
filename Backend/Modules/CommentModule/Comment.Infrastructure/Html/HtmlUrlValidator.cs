using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comment.Infrastructure.Html
{
    internal static class HtmlUrlValidator
    {
        public static bool IsSafe(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp ||
                    uri.Scheme == Uri.UriSchemeHttps)
                && !string.IsNullOrWhiteSpace(uri.Host);
        }
    }
}