using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xemio.Logic.Requests;
using Xemio.Logic.Services.JsonWebToken;

namespace Xemio.Logic.Services.Requests
{
    public class RequestManager : IRequestManager
    {
        private readonly IServiceProvider _serviceProvider;

        public RequestManager(IServiceProvider serviceProvider)
        {
            Guard.NotNull(serviceProvider, nameof(serviceProvider));
            
            this._serviceProvider = serviceProvider;
        }

        public IRequestContext StartRequestContext()
        {
            var scope = this._serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetService<IRequestContext>();

            return new ScopedRequestContext(scope, context);
        }

        private class ScopedRequestContext : IRequestContext
        {
            private readonly IServiceScope _scope;
            private readonly IRequestContext _actualContext;

            public ScopedRequestContext(IServiceScope scope, IRequestContext actualContext)
            {
                Guard.NotNull(scope, nameof(scope));
                Guard.NotNull(actualContext, nameof(actualContext));

                this._scope = scope;
                this._actualContext = actualContext;
            }

            public AuthToken CurrentUser
            {
                [DebuggerStepThrough]
                get => this._actualContext.CurrentUser;
                [DebuggerStepThrough]
                set => this._actualContext.CurrentUser = value;
            }

            [DebuggerStepThrough]
            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken token = default)
            {
                Guard.NotNull(request, nameof(request));

                return this._actualContext.Send(request, token);
            }

            [DebuggerStepThrough]
            public Task Publish<TNotification>(TNotification notification, CancellationToken token = default) where TNotification : INotification
            {
                Guard.NotNull(notification, nameof(notification));

                return this._actualContext.Publish(notification, token);
            }

            [DebuggerStepThrough]
            public Task CommitAsync(CancellationToken token = default)
            {
                return this._actualContext.CommitAsync(token);
            }

            [DebuggerStepThrough]
            public void Dispose()
            {
                this._scope.Dispose();
            }
        }
    }
}