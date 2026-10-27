using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.Core.Entity;
using Comment.Infrastructure.DataBase.ReadModels;
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
        public DbSet<CommentVote> commentVotes { get; set; }
        public DbSet<CommentAttachment> CommentAttachments => Set<CommentAttachment>();
        public DbSet<CommentAuthorReadModel> CommentAuthors => Set<CommentAuthorReadModel>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(CommentDbContext).Assembly);
        }

    }
}