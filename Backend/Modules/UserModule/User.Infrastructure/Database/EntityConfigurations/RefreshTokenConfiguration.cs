
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.Core.Entity;

namespace User.Infrastructure.Database.EntityConfigurations;

public sealed class RefreshTokenConfiguration
    : IEntityTypeConfiguration<RefreshTokenEntity>
{
    public void Configure(
        EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
