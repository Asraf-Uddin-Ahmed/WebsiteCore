using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsiteCore.Foundation.Core.Constant;

namespace WebsiteCore.WebApi.Authorization
{
    public class YearsOfExperienceAuthorizationHandler : AuthorizationHandler<YearsOfExperienceRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, YearsOfExperienceRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ApplicationClaims.EXPERIENCE))
                return Task.CompletedTask;

            int yearsOfExperience = int.Parse(context.User.FindFirst(c => c.Type == ApplicationClaims.EXPERIENCE).Value);
            if (yearsOfExperience >= requirement.YearsOfExperienceRequired)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
