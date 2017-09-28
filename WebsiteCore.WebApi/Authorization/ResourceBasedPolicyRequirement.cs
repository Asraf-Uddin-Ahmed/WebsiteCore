using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteCore.WebApi.Authorization
{
    public class ResourceBasedPolicyRequirement : IAuthorizationRequirement
    {
        public bool MustBeApproved { get; set; }
        public ResourceBasedPolicyRequirement(bool mustBeApproved)
        {
            MustBeApproved = mustBeApproved;
        }
    }
}
