using System;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Xemio.Logic.Requests.Auth;
using Xunit;

namespace Xemio.Logic.Tests.Requests
{
    public class AuthTests : RequestTests
    {
        [Fact]
        public async Task CanRegisterAndLogin()
        {
            var emailAddress = "haefele@test.de";
            var password = "12345678";

            using (var context = this.RequestManager.StartRequestContext())
            {
                var user = await context.Send(new RegisterUserRequest
                {
                    EmailAddress = emailAddress,
                    Password = password
                });

                await context.CommitAsync();

                Assert.NotNull(user);
                Assert.Equal(emailAddress, user.EmailAddress);
            }

            using (var context = this.RequestManager.StartRequestContext())
            {
                var token = await context.Send(new LoginUserRequest
                {
                    EmailAddress = emailAddress,
                    Password = password
                });

                await context.CommitAsync();

                Assert.NotNull(token);
            }
        }
    }
}
