using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xemio.Client.Data.Actions;
using Xemio.Logic.Entities;
using Xemio.Logic.Requests;
using Xemio.Logic.Requests.Auth.LoginUser;
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
        [Route("Login")]
        public async Task<ActionResult<string>> Login([FromBody]LoginUserAction action, CancellationToken token = default)
        {
            var loginUserRequest = new LoginUserRequest
            {
                EmailAddress = action.EmailAddress,
                Password = action.Password
            };

            var authToken = await this._requestContext.Send(loginUserRequest, token);

            await this._requestContext.CommitAsync(token);

            return this.Ok(new { Token = authToken.ToString() });
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<User>> Register([FromBody]RegisterUserAction action, CancellationToken token = default)
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