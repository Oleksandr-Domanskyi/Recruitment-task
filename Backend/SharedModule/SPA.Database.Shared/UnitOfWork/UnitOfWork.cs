using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SPA.Database.Shared.UnitOfWork
{
    public sealed class UnitOfWork<TDbContext, TRepository>(
        TDbContext dbContext,
        TRepository repository)
        : IUnitOfWork<TRepository>
        where TDbContext : DbContext
        where TRepository : class
    {
        public TRepository Repository { get; } = repository;

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}