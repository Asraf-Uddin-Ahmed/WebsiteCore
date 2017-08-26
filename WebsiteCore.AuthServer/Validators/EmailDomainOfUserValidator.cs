using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsiteCore.Foundation.Core.Entities.Auth;

namespace WebsiteCore.AuthServer.Validators
{
    public class EmailDomainOfUserValidator : IUserValidator<ApplicationUser>
    {
        readonly List<string> ALLOWED_EMAIL_DOMAINS = new List<string> { "outlook.com", "hotmail.com", "gmail.com", "yahoo.com" };

        public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            var emailDomain = user.Email.Split('@')[1];

            return Task.FromResult(ALLOWED_EMAIL_DOMAINS.Contains(emailDomain.ToLower()) ?
                IdentityResult.Success :
                IdentityResult.Failed(new IdentityError
                {
                    Code = "SuspicousEmailDomain",
                    Description = "Unauthorized email domain. We currently supporting: " + string.Join(", ", ALLOWED_EMAIL_DOMAINS)
                }));
        }
    }
}