using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using WebsiteCore.Foundation.Core.Entities.Auth;
using WebsiteCore.Foundation.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Models;
using IdentityServer4;
using IdentityServer4.Test;
using System.Security.Claims;
using IdentityModel;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using WebsiteCore.Foundation.Core.Constant;

namespace WebsiteCore.Foundation
{
    public class AuthConfig
    {
        public static void ConfigureServices(IServiceCollection services, IdentityBuilder identityBuilder, IIdentityServerBuilder identityServerBuilder, string connectionString)
        {
            var migrationsAssembly = typeof(AuthConfig).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            identityBuilder.AddEntityFrameworkStores<ApplicationDbContext, Guid>();

            identityServerBuilder
                .AddInMemoryPersistedGrants()
                /* Add-Migration InitialIdentityServerMigration -Context PersistedGrantDbContext -OutputDir Data/Migrations/IdentityServer/PersistedGrantDb */
                //.AddOperationalStore(builder =>
                //    builder.UseSqlServer(connectionString, options =>
                //        options.MigrationsAssembly(migrationsAssembly)))
                .AddInMemoryIdentityResources(AuthConfig.GetIdentityResources())
                .AddInMemoryApiResources(AuthConfig.GetApiResources())
                .AddInMemoryClients(AuthConfig.GetClients())
                /* Add-Migration InitialIdentityServerMigration -Context ConfigurationDbContext -OutputDir Data/Migrations/IdentityServer/ConfigurationDb */
                //.AddConfigurationStore(builder =>
                //    builder.UseSqlServer(connectionString, options =>
                //        options.MigrationsAssembly(migrationsAssembly)))
                .AddAspNetIdentity<ApplicationUser>();

        }
        public static void ConfigureApp(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDatabaseErrorPage();
            }
        }
        public static void InitializeConfigurationDb(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if (!context.Clients.Any())
                {
                    foreach (var client in GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
        public static void InitializeAuthDb(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                PopulateRolesTable(roleManager);
                roleManager.Dispose();

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                PopulateUserTable(userManager);
                userManager.Dispose();

            }
        }



        private static void PopulateRolesTable(RoleManager<ApplicationRole> roleManager)
        {
            if (!roleManager.Roles.Any(r => r.Name == ApplicationRoles.ADMIN))
            {
                roleManager.CreateAsync(new ApplicationRole { Name = ApplicationRoles.ADMIN }).Wait();
            }
            if (!roleManager.Roles.Any(r => r.Name == ApplicationRoles.DEVELOPER))
            {
                roleManager.CreateAsync(new ApplicationRole { Name = ApplicationRoles.DEVELOPER }).Wait();
            }
            if (!roleManager.Roles.Any(r => r.Name == ApplicationRoles.EMPLOYEE))
            {
                roleManager.CreateAsync(new ApplicationRole { Name = ApplicationRoles.EMPLOYEE }).Wait();
            }
        }
        private static void PopulateUserTable(UserManager<ApplicationUser> userManager)
        {
            if (!userManager.Users.Any(r => r.UserName == "DeveloperUser"))
            {
                var superAdminUser = new ApplicationUser()
                {
                    UserName = "DeveloperUser",
                    Email = "13ratul@gmail.com",
                    EmailConfirmed = true
                };
                userManager.CreateAsync(superAdminUser, "dP@ssword123").Wait();
                userManager.AddToRoleAsync(superAdminUser, ApplicationRoles.DEVELOPER).Wait();
            }

            if (!userManager.Users.Any(r => r.UserName == "AdminUser"))
            {
                var adminUser = new ApplicationUser()
                {
                    UserName = "AdminUser",
                    Email = "13ratul+admin@gmail.com",
                    EmailConfirmed = true
                };
                userManager.CreateAsync(adminUser, "aP@ssword123").Wait();
                userManager.AddToRoleAsync(adminUser, ApplicationRoles.ADMIN).Wait();
            }

            if (!userManager.Users.Any(r => r.UserName == "EmployeeUser"))
            {
                var appUser = new ApplicationUser()
                {
                    UserName = "EmployeeUser",
                    Email = "13ratul+employee@gmail.com",
                    EmailConfirmed = true
                };
                userManager.CreateAsync(appUser, "uP@ssword123").Wait();
                userManager.AddToRoleAsync(appUser, ApplicationRoles.EMPLOYEE).Wait();
            }
        }
        // scopes define the resources in your system
        private static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource {
                    Name = "role",
                    UserClaims = new List<string> {"role"}
                }
            };
        }

        private static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API")
                {
                    UserClaims = {"role"}
                },
                new ApiResource {
                    Name = "customAPI",
                    DisplayName = "Custom API",
                    Description = "Custom API Access",
                    UserClaims = new List<string> {"role"},
                    ApiSecrets = new List<Secret> {new Secret("scopeSecret".Sha256())},
                    Scopes = new List<Scope> {
                        new Scope("customAPI.read"),
                        new Scope("customAPI.write")
                    }
                }
            };
        }

        // clients want to access resources (aka scopes)
        private static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1" }
                },

                // resource owner password grant client
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1" }
                },

                // OpenID Connect hybrid flow and client credentials client (MVC)
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    RequireConsent = true,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = { "http://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },
                    AllowOfflineAccess = true
                },
                // OpenID Connect authorization code flow (POSTMAN)
                new Client
                {
                    ClientId = "postman",
                    ClientName = "Postman Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,

                    RedirectUris = { "https://www.getpostman.com/oauth2/callback" },
                    PostLogoutRedirectUris = { "https://www.getpostman.com" },
                    AllowedCorsOrigins = { "https://www.getpostman.com" },

                    EnableLocalLogin = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "api1"
                    },
                    ClientSecrets = { new Secret("secret".Sha256()) }
                }
            };
        }

    }
}
