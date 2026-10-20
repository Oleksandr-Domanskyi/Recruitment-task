using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.Core.Entity;

namespace User.Infrastructure.Database.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");

        builder.Property(x => x.AvatarUrl)
            .HasMaxLength(2048);

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}