
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using CommentEntity = Comment.Core.Entity.Comment;

namespace Comment.Infrastructure.DataBase.EntityConfigurations;

public class CommentEntityConfiguration
    : IEntityTypeConfiguration<CommentEntity>
{
    public void Configure(EntityTypeBuilder<CommentEntity> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired(false);

        builder.Property(x => x.Text)
            .IsRequired();

        builder.Property(x => x.ImageUrl)
            .IsRequired(false)
            .HasMaxLength(2048);

        builder.Property(x => x.Rating)
            .HasDefaultValue(0);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne<CommentEntity>()
            .WithMany(x => x.Replies)
            .HasForeignKey(x => x.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(x => x.Replies)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ParentCommentId);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.Rating);
    }
}
