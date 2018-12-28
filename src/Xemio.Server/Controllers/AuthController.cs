using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xemio.Client.Data.Actions;
using Xemio.Logic.Requests;
using Xemio.Logic.Requests.Auth.RegisterUser;

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
        public async Task<ActionResult<object>> Register([FromBody]RegisterUserAction action, CancellationToken token = default)
        {
            var registerUserRequest = new RegisterUserRequest
            {
                EmailAddress = action.EmailAddress,
                Password = action.Password
            };

            var user = await this._requestContext.Send(registerUserRequest, token).ConfigureAwait(false);

            await this._requestContext.CommitAsync(token);

            return this.Ok(user);
        }
    }
}