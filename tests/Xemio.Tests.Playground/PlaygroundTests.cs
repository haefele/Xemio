using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xemio.Logic;
using Xemio.Logic.Requests.Auth;
using Xemio.Logic.Services.JsonWebToken;
using Xemio.Logic.Services.Requests;

namespace Xemio.Tests.Playground
{
    public abstract class PlaygroundTests : IDisposable
    {
        protected ServiceProvider ServiceProvider { get; }
        protected IRequestManager RequestManager => this.ServiceProvider.GetService<IRequestManager>();

        public PlaygroundTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Database:Urls"] = "http://127.0.0.1:8080",
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

        protected async Task<AuthToken> CreateUserAndLogin()
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

                return token;
            }
        }

        public void Dispose()
        {
            this.ServiceProvider.Dispose();
        }
    }
}