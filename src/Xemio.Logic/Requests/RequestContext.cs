using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents.Session;
using Xemio.Logic.Entities;
using Xemio.Logic.Services.JsonWebToken;

namespace Xemio.Logic.Requests
{
    public class RequestContext : IRequestContext
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IMediator _mediator;

        public RequestContext(IAsyncDocumentSession session, IMediator mediator)
        {
            Guard.NotNull(session, nameof(session));
            Guard.NotNull(mediator, nameof(mediator));

            this._session = session;
            this._mediator = mediator;
        }

        public AuthToken CurrentUser { get; set; }

        [DebuggerStepThrough]
        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken token = default)
        {
            Guard.NotNull(request, nameof(request));

            return await this._mediator.Send(request, token).ConfigureAwait(false);
        }

        [DebuggerStepThrough]
        public async Task Publish<TNotification>(TNotification notification, CancellationToken token = default) where TNotification : INotification
        {
            Guard.NotNull(notification, nameof(notification));

            await this._mediator.Publish(notification, token).ConfigureAwait(false);
        }

        public async Task CommitAsync(CancellationToken token = default)
        {
            await this._session.SaveChangesAsync(token);
        }

        public void Dispose()
        {
        }
    }
}