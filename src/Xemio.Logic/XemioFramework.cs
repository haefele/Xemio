using System;
using System.Linq;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Embedded;
using Xemio.Logic.Configuration;
using Xemio.Logic.Requests;
using Xemio.Logic.Services;

namespace Xemio.Logic
{
    public static class XemioFramework
    {
        public static void AddXemioFramework(this IServiceCollection self, IConfiguration configuration)
        {
            Guard.NotNull(self, nameof(self));
            Guard.NotNull(configuration, nameof(configuration));

            self.AddLogging(f =>
            {
                f.AddDebug();
            });
            self.AddConfiguration(configuration);
            self.AddDatabase();
            self.AddValidators();
            self.AddMediatR();
            self.AddServices();
        }

        private static void AddConfiguration(this IServiceCollection self, IConfiguration configuration)
        {
            Guard.NotNull(self, nameof(self));
            Guard.NotNull(configuration, nameof(configuration));

            self.Configure<DatabaseConfiguration>(configuration.GetSection("Database"));
            self.Configure<ServerConfiguration>(configuration.GetSection("Server"));
            self.Configure<CryptoConfiguration>(configuration.GetSection("Crypto"));
        }

        private static void AddDatabase(this IServiceCollection self)
        {
            Guard.NotNull(self, nameof(self));

            self.AddSingleton<IDocumentStore>(f =>
            {
                var databaseConfiguration = f.GetRequiredService<IOptions<DatabaseConfiguration>>().Value;

                if (databaseConfiguration.UseEmbeddedTestServer == false)
                {
                    var store = new DocumentStore();
                    store.Database = databaseConfiguration.DatabaseName;
                    store.Urls = databaseConfiguration.Urls.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
                    
                    store.Initialize();

                    return store;
                }
                else
                {
                    string databaseName = databaseConfiguration.CreateRandomDatabaseNameForEmbeddedUsage
                        ? Guid.NewGuid().ToString("N")
                        : databaseConfiguration.DatabaseName;

                    EmbeddedServer.Instance.StartServer();
                    return EmbeddedServer.Instance.GetDocumentStore(databaseName);
                }
            });

            self.AddScoped<IAsyncDocumentSession>(f => f.GetRequiredService<IDocumentStore>().OpenAsyncSession());
        }

        private static void AddValidators(this IServiceCollection self)
        {
            Guard.NotNull(self, nameof(self));
            
            self.Scan(scan =>
            {
                scan.FromAssemblies(typeof(XemioFramework).Assembly)
                    .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
                    .As(type => type.GetInterfaces().Where(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)))
                    .WithTransientLifetime();
            });
        }

        private static void AddMediatR(this IServiceCollection self)
        {
            Guard.NotNull(self, nameof(self));
            
            self.AddMediatR(typeof(XemioFramework).Assembly);

            self.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            self.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

            self.Scan(scan =>
            {
                scan.FromAssemblies(typeof(XemioFramework).Assembly)
                    .AddClasses(classes => classes.AssignableTo(typeof(IPipelineBehavior<,>)))
                    .As(typeof(IPipelineBehavior<,>))
                    .WithTransientLifetime();
            });

            self.AddScoped<IRequestContext, RequestContext>();
        }

        private static void AddServices(this IServiceCollection self)
        {
            Guard.NotNull(self, nameof(self));

            self.Scan(scan =>
            {
                scan.FromAssemblies(typeof(XemioFramework).Assembly)
                    .AddClasses(classes => classes
                        .InNamespaces(typeof(ServicesPlaceholder).Namespace)
                        .Where(f => f.Name.EndsWith("Service") || f.Name.EndsWith("Manager") || f.Name.EndsWith("Generator")))
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime();
            });
        }
    }
}