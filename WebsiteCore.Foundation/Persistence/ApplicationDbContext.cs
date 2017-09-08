using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebsiteCore.Foundation.Core.Entities;
using WebsiteCore.Foundation.Core.Entities.Auth;
using WebsiteCore.Foundation.Persistence.EntityConfigurations;
using WebsiteCore.Foundation.Microsoft.EntityFrameworkCore;

namespace WebsiteCore.Foundation.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {

        public DbSet<Settings> Settings { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            modelBuilder.AddConfiguration(new SettingsConfiguration());

        }
    }
}
