using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xemio.Logic.Entities;
using Xemio.Logic.Requests;

namespace Xemio.Logic.Services.Requests
{
    public class RequestManager : IRequestManager
    {
        private readonly IServiceProvider _serviceProvider;

        public RequestManager(IServiceProvider serviceProvider)
        {
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
                this._scope = scope;
                this._actualContext = actualContext;
            }

            public User CurrentUser
            {
                get { return this._actualContext.CurrentUser; }
                set { this._actualContext.CurrentUser = value; }
            }

            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken token = default(CancellationToken))
            {
                return this._actualContext.Send(request, token);
            }

            public Task Publish<TNotification>(TNotification notification, CancellationToken token = default(CancellationToken)) where TNotification : INotification
            {
                return this._actualContext.Publish(notification, token);
            }

            public Task CommitAsync(CancellationToken token = default(CancellationToken))
            {
                return this._actualContext.CommitAsync(token);
            }

            public void Dispose()
            {
                this._scope.Dispose();
            }
        }
    }
}