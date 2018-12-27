using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents.Session;

namespace Xemio.Logic.Requests
{
    public class RequestContext : IRequestContext
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IMediator _mediator;

        public RequestContext(IAsyncDocumentSession session, IMediator mediator)
        {
            this._session = session;
            this._mediator = mediator;
        }

        public User CurrentUser { get; set; }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken token = default(CancellationToken))
        {
            return await this._mediator.Send(request, token).ConfigureAwait(false);
        }

        public async Task Publish<TNotification>(TNotification notification, CancellationToken token = default(CancellationToken)) where TNotification : INotification
        {
            await this._mediator.Publish(notification, token).ConfigureAwait(false);
        }

        public async Task CommitAsync(CancellationToken token = default(CancellationToken))
        {
            await this._session.SaveChangesAsync(token);
        }

        public void Dispose()
        {
        }
    }
}