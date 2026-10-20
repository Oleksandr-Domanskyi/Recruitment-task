using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace User.Infrastructure.Database.EntityConfigurations
{
    public sealed class RoleClaimConfiguration
        : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
        {
            builder.ToTable("RoleClaims");
        }
    }
}