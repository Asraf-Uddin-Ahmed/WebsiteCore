using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteCore.AuthServer.Validators
{
    public static class ApplicationUserValidator
    {
        public static UserOptions Configure()
        {
            // For detail visit: https://github.com/aspnet/Identity/blob/c8a276e716064ac40df5a56512e0217b14b48060/src/Microsoft.AspNetCore.Identity/UserOptions.cs
            return new UserOptions
            {
                // Use 'null' for support all character
                AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+",
                RequireUniqueEmail = true
            };
        }
    }
}
