using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPA.Database.Shared.UnitOfWork
{
    public interface IUnitOfWork<out TRepository> where TRepository : class
    {
        TRepository Repository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}