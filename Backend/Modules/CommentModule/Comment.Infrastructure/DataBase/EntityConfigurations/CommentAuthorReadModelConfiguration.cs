using Comment.Infrastructure.DataBase.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comment.Infrastructure.DataBase.EntityConfigurations;

public sealed class CommentAuthorReadModelConfiguration
    : IEntityTypeConfiguration<CommentAuthorReadModel>
{
    public void Configure(EntityTypeBuilder<CommentAuthorReadModel> builder)
    {
        builder.ToTable("Users", "users", t => t.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserName);
        builder.Property(x => x.Email);
    }
}