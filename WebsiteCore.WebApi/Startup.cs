// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebsiteCore.Foundation.Core.Constant;
using WebsiteCore.WebApi.Authorization;

namespace WebsiteCore.WebApi
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddAuthorization(option =>
                {
                    option.AddPolicy(ApplicationPolicies.DEVELOPER_ONLY, 
                        policy => policy.RequireClaim("role", ApplicationRoles.DEVELOPER));

                    option.AddPolicy(ApplicationPolicies.YEARS_OF_EXEPERIENCE, 
                        policy => policy.AddRequirements(new YearsOfExperienceRequirement(6)));
                })
                .AddJsonFormatters();

            services.AddSingleton<IAuthorizationHandler, YearsOfExperienceAuthorizationHandler>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = "http://localhost:5000",
                RequireHttpsMetadata = false,

                ApiName = "api1"
            });

            app.UseMvc();
        }
    }
}