using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace User.Infrastructure.Database.EntityConfigurations;

public class RoleConfiguration
    : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.ToTable("Roles");
    }
}