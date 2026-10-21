using Comment.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comment.Infrastructure.DataBase.EntityConfigurations;

public sealed class CommentAttachmentConfiguration
    : IEntityTypeConfiguration<CommentAttachment>
{
    public void Configure(EntityTypeBuilder<CommentAttachment> builder)
    {
        builder.ToTable("CommentAttachments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.BlobName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.OriginalFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.FileSize)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(x => x.Comment)
            .WithMany(x => x.Attachments)
            .HasForeignKey(x => x.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.CommentId);
    }
}