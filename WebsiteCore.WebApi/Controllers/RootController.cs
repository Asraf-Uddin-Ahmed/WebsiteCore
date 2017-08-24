using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebsiteCore.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("/")]
    public class RootController : Controller
    {
        // GET: Root
        [HttpGet]
        public string Get()
        {
            return "API is running...";
        }
        
    }
}
