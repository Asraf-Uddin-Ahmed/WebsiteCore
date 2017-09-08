using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebsiteCore.Foundation.Core.Entities;
using WebsiteCore.Foundation.Microsoft.EntityFrameworkCore;

namespace WebsiteCore.Foundation.Persistence.EntityConfigurations
{
    public abstract class EntityConfiguration<TEntity> : EntityTypeConfiguration<TEntity>
          where TEntity : Entity
    {
        public abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);

        public override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(e => e.Id)
                .IsRequired();

            ConfigureEntity(builder);
        }
    }
}
