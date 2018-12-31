using System.Threading.Tasks;
using Xemio.Logic.Requests;
using Xemio.Logic.Requests.Auth.LoginUser;
using Xemio.Logic.Requests.Auth.RegisterUser;
using Xemio.Logic.Services.JsonWebToken;
using Xunit;

namespace Xemio.Tests.Playground
{
    public class RequestAuthorizationTests : PlaygroundTests
    {
        [Fact]
        public async Task Playground()
        {
            string token;

            using (var context = this.RequestManager.StartRequestContext())
            {
                var result = await context.Send(new RegisterUserRequest
                {
                    EmailAddress = "haefele@xemio.net",
                    Password = "12345678"
                });

                await context.CommitAsync();

                token = result.ToString();
            }

            using (var context = this.RequestManager.StartRequestContext())
            {
                context.CurrentUser = new AuthToken(token);

                var result = await context.Send(new LoginUserRequest
                {
                    EmailAddress = "haefele@xemio.net",
                    Password = "12345678"
                });
            }
        }
    }
}
