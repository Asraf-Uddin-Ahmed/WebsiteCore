using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsiteCore.Foundation.Core.Entities.Auth;

namespace WebsiteCore.AuthServer.Validators
{
    public class CommonlyUsedPasswordValidator : IPasswordValidator<ApplicationUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager,
                                                  ApplicationUser user,
                                                  string password)
        {
            return Task.FromResult(password.Contains("abcdef") 
                || password.Contains("123456") 
                || password.Contains("password") ? 
                IdentityResult.Failed(new IdentityError
                {
                    Code = "CommonlyUsedPassword",
                    Description = "This is most commonly used password. Password can not contain this sequence of characters."
                }) :
                IdentityResult.Success);
        }
    }
}
