using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebsiteCore.Foundation.Core.Constant;

namespace WebsiteCore.WebApi.Controllers
{
    [Route("[controller]")]
    public class ValuesController : Controller
    {
        [Authorize(Roles = ApplicationRoles.ADMIN)]
        [Route("admin")]
        [HttpGet]
        public string GetForAdmin()
        {
            return "This is from Admin";
        }
        [Authorize(Roles = ApplicationRoles.DEVELOPER)]
        [Route("developer")]
        [HttpGet]
        public string GetForDeveloper()
        {
            return "This is from Developer";
        }
        [Authorize(Policy = ApplicationPolicies.DEVELOPER_ONLY)]
        [Route("developerpolicy")]
        [HttpGet]
        public string GetForDeveloperPolicy()
        {
            return "This is from Developer POLICY";
        }
        [Authorize(Policy = ApplicationPolicies.YEARS_OF_EXEPERIENCE)]
        [Route("yearsofexeperiencepolicy")]
        [HttpGet]
        public string GetForYearsOfExeperiencePolicy()
        {
            return "This is from YEARS_OF_EXEPERIENCE POLICY";
        }
        [Authorize(Roles = ApplicationRoles.EMPLOYEE)]
        [Route("employee")]
        [HttpGet]
        public string GetForEmployee()
        {
            return "This is from Employee";
        }
        [Authorize]
        [Route("authorize")]
        [HttpGet]
        public string GetForAuthorized()
        {
            return "This is from Authorized";
        }
        // GET values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
