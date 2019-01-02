using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Xemio.Logic.Requests.Auth.LoginUser;
using Xemio.Logic.Requests.Auth.RegisterUser;
using Xemio.Logic.Services.Requests;

namespace Xemio.Logic.Tests
{
    public abstract class RequestTests : IDisposable
    {
        protected ServiceProvider ServiceProvider { get; }
        protected IRequestManager RequestManager => this.ServiceProvider.GetService<IRequestManager>();
        protected IMapper Mapper => this.ServiceProvider.GetService<IMapper>();

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

        protected async Task WaitForUserToContinueTheTest()
        {
            var store = this.ServiceProvider.GetService<IDocumentStore>();
            var databaseNameEncoded = Uri.EscapeDataString(store.Database);

            var documentsPage = store.Urls[0] + "/studio/index.html#databases/documents?&database=" + databaseNameEncoded + "&withStop=true";

            Process.Start(new ProcessStartInfo("cmd", $"/c start \"Stop & look at Studio\" \"{documentsPage}\""));

            do
            {
                await Task.Delay(500);

                using (var session = store.OpenSession())
                {
                    if (session.Advanced.Exists("Debug/Done"))
                    {
                        session.Delete("Debug/Done");
                        session.SaveChanges();

                        break;
                    }
                }
            } while (true);
        }

        public void Dispose()
        {
            this.ServiceProvider.Dispose();
        }
    }
}