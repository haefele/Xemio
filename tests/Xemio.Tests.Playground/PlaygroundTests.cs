using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xemio.Logic;
using Xemio.Logic.Services.Requests;

namespace Xemio.Tests.Playground
{
    public abstract class PlaygroundTests
    {
        protected IServiceProvider ServiceProvider { get; }
        protected IRequestManager RequestManager => this.ServiceProvider.GetService<IRequestManager>();

        public PlaygroundTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Database:Urls"] = "http://127.0.0.1:8080",
                    ["Database:DatabaseName"] = "XemioTest",

                    ["Crypto:AuthTokenSecret"] = "123456789123456789123456789"
                })
                .Build();

            var collection = new ServiceCollection();
            collection.AddXemioFramework(configuration);

            this.ServiceProvider = collection.BuildServiceProvider();
        }
    }
}