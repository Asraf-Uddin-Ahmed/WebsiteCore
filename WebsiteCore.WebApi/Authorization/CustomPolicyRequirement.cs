using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteCore.WebApi.Authorization
{
    public class CustomPolicyRequirement: IAuthorizationRequirement
    {
        public int YearsOfExperienceRequired { get; set; }
        public CustomPolicyRequirement(int yearsOfExperienceRequired)
        {
            YearsOfExperienceRequired = yearsOfExperienceRequired;
        }
    }
}
