using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xemio.Logic.Requests.Auth.LoginUser;
using Xemio.Logic.Requests.Auth.RegisterUser;
using Xemio.Logic.Services.Requests;

namespace Xemio.Logic.Tests
{
    public abstract class RequestTests
    {
        protected ServiceProvider ServiceProvider { get; }
        protected IRequestManager RequestManager => this.ServiceProvider.GetService<IRequestManager>();

        public RequestTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Database:DatabaseName"] = "XemioTest",
                    ["Database:UseEmbeddedTestServer"] = "true",
                    ["Database:CreateRandomDatabaseNameForEmbeddedUsage"] = "true",

                    ["Crypto:AuthTokenSecret"] = "123456789123456789123456789"
                })
                .Build();

            var collection = new ServiceCollection();
            collection.AddXemioFramework(configuration);

            this.ServiceProvider = collection.BuildServiceProvider();
        }

        protected async Task<string> CreateUserAndLogin()
        {
            using (var context = this.RequestManager.StartRequestContext())
            {
                await context.Send(new RegisterUserRequest
                {
                    EmailAddress = "haefele@xemio.net",
                    Password = "12345678"
                });

                await context.CommitAsync();
            }

            using (var context = this.RequestManager.StartRequestContext())
            {
                var token = await context.Send(new LoginUserRequest
                {
                    EmailAddress = "haefele@xemio.net",
                    Password = "12345678"
                });

                await context.CommitAsync();

                return token.ToString();
            }
        }

        public void Dispose()
        {
            this.ServiceProvider.Dispose();
        }
    }
}