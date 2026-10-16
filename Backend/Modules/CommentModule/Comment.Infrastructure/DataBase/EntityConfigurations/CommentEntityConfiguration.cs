using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comment.Infrastructure.DataBase.EntityConfigurations
{
    public class CommentEntityConfiguration : IEntityTypeConfiguration<Comment.Core.Entity.Comment>
    {
        public void Configure(EntityTypeBuilder<Core.Entity.Comment> builder)
        {
            throw new NotImplementedException();
        }
    }
}