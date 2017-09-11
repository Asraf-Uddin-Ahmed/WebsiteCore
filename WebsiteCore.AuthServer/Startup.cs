using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebsiteCore.AuthServer.Models;
using WebsiteCore.AuthServer.Services;
using System.Reflection;
using WebsiteCore.Foundation.Persistence;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Identity;
using WebsiteCore.Foundation;
using WebsiteCore.AuthServer.Validators;
using WebsiteCore.Foundation.Core.Entities.Auth;

namespace WebsiteCore.AuthServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            IdentityBuilder identityBuilder = services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password = ApplicationPasswordValidator.Configure();
                    options.User = ApplicationUserValidator.Configure();
                })
                .AddPasswordValidator<SameCharacterPasswordValidator>()
                .AddPasswordValidator<CommonlyUsedPasswordValidator>()
                .AddUserValidator<EmailDomainOfUserValidator>()
                .AddDefaultTokenProviders();

            IIdentityServerBuilder identityServerBuilder = services
                .AddIdentityServer()
                //.AddSigningCredential()
                .AddTemporarySigningCredential();
            
            AuthConfig.ConfigureServices(services, identityBuilder, identityServerBuilder, connectionString);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();
            app.UseIdentityServer();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            app.UseGoogleAuthentication(new GoogleOptions
            {
                AuthenticationScheme = "Google",
                SignInScheme = "Identity.External", // this is the name of the cookie middleware registered by UseIdentity()
                ClientId = "998042782978-s07498t8i8jas7npj4crve1skpromf37.apps.googleusercontent.com",
                ClientSecret = "HsnwJri_53zn7VcO1Fm7THBb",
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            AuthConfig.ConfigureApp(app, env);
            AuthConfig.InitializeAuthDb(app);

            DbSeeder.Seed(app);

        }
    }
}
