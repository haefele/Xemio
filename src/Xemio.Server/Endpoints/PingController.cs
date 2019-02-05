using Microsoft.AspNetCore.Mvc;
using Raven.Embedded;

namespace Xemio.Server.Endpoints
{
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase 
    {
        [HttpGet]
        public string Get()  
        {
            return "Pong";
        }

        [HttpPost]
        public string Post() 
        {
            return "Pong";
        }
    }
}