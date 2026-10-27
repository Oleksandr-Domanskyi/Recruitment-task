using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comment.Application.Interfaces
{
    public interface ICommentCache
    {
        Task<T> GetOrCreateAsync<T>(
            string key,
            Func<CancellationToken, Task<T>> factory,
            CancellationToken cancellationToken = default,
            Func<T, bool>? shouldCache = null
        );

        void Invalidate();
    }
}