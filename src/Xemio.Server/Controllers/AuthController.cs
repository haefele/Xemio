using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xemio.Logic.Requests;
using Xemio.Logic.Services.RegisterUser;

namespace Xemio.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IRequestContext _requestContext;

        public AuthController(IRequestContext requestContext)
        {
            this._requestContext = requestContext;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<object>> Register()
        {
            var user = await this._requestContext.Send(new RegisterUserRequest
            {
                EmailAddress = "haefele@xemio.net",
                Password = "12345678"
            }).ConfigureAwait(false);

            return user;
        }
    }
}