using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsiteCore.Foundation.Core.Entities.Auth;

namespace WebsiteCore.AuthServer.Validators
{
    public class SameCharacterPasswordValidator : IPasswordValidator<ApplicationUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager,
                                                  ApplicationUser user,
                                                  string password)
        {
            return Task.FromResult(password.Distinct().Count() == 1 ?
                IdentityResult.Failed(new IdentityError
                {
                    Code = "SameCharacterPassword",
                    Description = "Passwords cannot be all the same character."
                }) :
                IdentityResult.Success);
        }
    }
}
