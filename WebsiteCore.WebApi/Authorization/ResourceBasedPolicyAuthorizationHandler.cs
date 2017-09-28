using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsiteCore.Foundation.Core.Constant;
using WebsiteCore.WebApi.Models.Request.Value;

namespace WebsiteCore.WebApi.Authorization
{
    public class ResourceBasedPolicyAuthorizationHandler : AuthorizationHandler<ResourceBasedPolicyRequirement, ResourceBasedPolicyRequestModel>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ResourceBasedPolicyRequirement requirement, ResourceBasedPolicyRequestModel resource)
        {
            if (requirement.MustBeApproved == resource.Approved)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
