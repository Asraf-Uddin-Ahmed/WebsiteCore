using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteCore.AuthServer.Validators
{
    public static class ApplicationPasswordValidator
    {
        public static PasswordOptions Configure()
        {
            // For detail visit: https://github.com/aspnet/Identity/blob/c8a276e716064ac40df5a56512e0217b14b48060/src/Microsoft.AspNetCore.Identity/PasswordOptions.cs
            return new PasswordOptions
            {
                RequireDigit = true,
                RequiredLength = 6,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };
        }
    }
}
