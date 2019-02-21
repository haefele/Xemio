using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xemio.Client.Data.Endpoints.Auth;
using Xemio.Logic.Requests;
using Xemio.Logic.Requests.Auth;

namespace Xemio.Server.Endpoints
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

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResult>> Login(LoginAction action, CancellationToken token)
        {
            var request = new LoginUserRequest
            {
                EmailAddress = action.EmailAddress,
                Password = action.Password
            };

            var authToken = await this._requestContext.Send(request, token);

            await this._requestContext.CommitAsync(token);

            return this.Ok(new LoginResult {Token = authToken.ToString()});
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(RegisterAction action, CancellationToken token)
        {
            var request = new RegisterUserRequest
            {
                EmailAddress = action.EmailAddress,
                Password = action.Password
            };

            await this._requestContext.Send(request, token).ConfigureAwait(false);

            await this._requestContext.CommitAsync(token);

            return this.Ok();
        }
    }
}