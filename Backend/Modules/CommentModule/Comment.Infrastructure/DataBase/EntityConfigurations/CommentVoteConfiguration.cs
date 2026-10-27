using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comment.Infrastructure.DataBase.EntityConfigurations
{
    public sealed class CommentVoteConfiguration
        : IEntityTypeConfiguration<CommentVote>
    {
        public void Configure(EntityTypeBuilder<CommentVote> builder)
        {
            builder.ToTable("CommentVotes");

            builder.HasKey(x => new
            {
                x.CommentId,
                x.UserId
            });

            builder.Property(x => x.Value)
                .IsRequired();

            builder.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_CommentVote_Value",
                    "\"Value\" IN (-1, 1)"
                ));

            builder.HasOne<Comment.Core.Entity.Comment>()
                .WithMany()
                .HasForeignKey(x => x.CommentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}