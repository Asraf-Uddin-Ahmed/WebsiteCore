using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebsiteCore.Foundation.Core.Entities;

namespace WebsiteCore.Foundation.Persistence.EntityConfigurations
{
    public class SettingsConfiguration : EntityConfiguration<Settings>
    {
        public override void ConfigureEntity(EntityTypeBuilder<Settings> builder)
        {
            builder.Property(s => s.DisplayName)
                .IsRequired();

            builder.Property(s => s.Name)
                .IsRequired();

            builder.Property(s => s.Type)
                .IsRequired();

            builder.Property(s => s.Value)
                .IsRequired();

        }
    }
}
