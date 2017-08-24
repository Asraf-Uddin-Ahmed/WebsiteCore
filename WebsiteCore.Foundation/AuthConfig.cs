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

namespace WebsiteCore.Foundation
{
    public class AuthConfig
    {
        public static IdentityBuilder ConfigureServices(IServiceCollection services, IIdentityServerBuilder identityServerBuilder, string connectionString)
        {
            var migrationsAssembly = typeof(AuthConfig).GetTypeInfo().Assembly.GetName().Name;

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            IdentityBuilder identityBuilder = services
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

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

            return identityBuilder;
        }

        public static void InitializeConfigurationDb(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

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
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
                
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                if (!userManager.Users.Any())
                {
                    foreach (var testUser in GetUsers())
                    {
                        var identityUser = new IdentityUser(testUser.Username)
                        {
                            Id = testUser.SubjectId
                        };

                        foreach (var claim in testUser.Claims)
                        {
                            identityUser.Claims.Add(new IdentityUserClaim<string>
                            {
                                UserId = identityUser.Id,
                                ClaimType = claim.Type,
                                ClaimValue = claim.Value,
                            });
                        }

                        userManager.CreateAsync(identityUser, testUser.Password).Wait();
                    }
                }
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
                new ApiResource("api1", "My API"),
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
                }
            };
        }
        private static List<TestUser> GetUsers()
        {
            return new List<TestUser> {
                new TestUser {
                    SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                    Username = "scott",
                    Password = "password",
                    Claims = new List<Claim> {
                        new Claim(JwtClaimTypes.Email, "scott@scottbrady91.com"),
                        new Claim(JwtClaimTypes.Role, "admin")
                    }
                }
            };
        }

    }
}
