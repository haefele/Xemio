using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xemio.Client.Data.Actions;
using Xemio.Logic.Database.Entities;
using Xemio.Logic.Requests;
using Xemio.Logic.Requests.Auth.LoginUser;
using Xemio.Logic.Requests.Auth.RegisterUser;

namespace Xemio.Server.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRequestContext _requestContext;

        public AuthController(IRequestContext requestContext)
        {
            this._requestContext = requestContext;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<string>> Login(LoginUserAction action, CancellationToken token)
        {
            var request = new LoginUserRequest
            {
                EmailAddress = action.EmailAddress,
                Password = action.Password
            };

            var authToken = await this._requestContext.Send(request, token);

            await this._requestContext.CommitAsync(token);

            return this.Ok(new { Token = authToken.ToString() });
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult> Register(RegisterUserAction action, CancellationToken token)
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