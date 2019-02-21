using System;
using System.Linq;
using AutoMapper;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions;
using Raven.Client.ServerWide.Operations;
using Raven.Embedded;
using Xemio.Logic.Configuration;
using Xemio.Logic.Requests;
using Xemio.Logic.Services;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

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
            self.AddMapper();
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
                    var serverOptions = new ServerOptions();
                    serverOptions.FrameworkVersion = "2.2.1"; //TODO: Find a better way for this
                    
                    EmbeddedServer.Instance.StartServer(serverOptions);

                    string databaseName = databaseConfiguration.CreateRandomDatabaseNameForEmbeddedUsage
                        ? Guid.NewGuid().ToString("N")
                        : databaseConfiguration.DatabaseName;

                    // Little workaround for disposing order issue
                    // SkipCreatingDatabase because we have to attach to store.AfterDispose before store.Maintenance internally subscribes to that event
                    // And that event subscription happens the first time we use the store.Maintenance instance
                    var databaseOptions = new DatabaseOptions(databaseName)
                    {
                        SkipCreatingDatabase = true,
                    };
                    var store = EmbeddedServer.Instance.GetDocumentStore(databaseOptions);

                    if (databaseConfiguration.CreateRandomDatabaseNameForEmbeddedUsage)
                    {
                        store.AfterDispose += (s, e) =>
                        {
                            store.Maintenance.Server.Send(new DeleteDatabasesOperation(databaseName, hardDelete: true));
                        };
                    }

                    try 
                    {
                        store.Maintenance.Server.Send(new CreateDatabaseOperation(databaseOptions.DatabaseRecord));
                    }
                    catch (ConcurrencyException) 
                    {
                        // The database already exists, move on
                    }


                    IndexCreation.CreateIndexes(typeof(XemioFramework).Assembly, store);

                    return store;
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

        private static void AddMapper(this IServiceCollection self)
        {
            Guard.NotNull(self, nameof(self));

            self.AddAutoMapper();

            self.BuildServiceProvider().GetService<IConfigurationProvider>().AssertConfigurationIsValid();
        }
    }
}