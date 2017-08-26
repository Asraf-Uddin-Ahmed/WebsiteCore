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
