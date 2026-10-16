using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SPA.Database.Shared.Database;

namespace Comment.Infrastructure.DataBase
{
    public class CommentDbContext : SPADbContext
    {
        protected override string Schema => "comment";

        public CommentDbContext(DbContextOptions<CommentDbContext> options) : base(options)
        {
        }
        public DbSet<Comment.Core.Entity.Comment> Comments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}