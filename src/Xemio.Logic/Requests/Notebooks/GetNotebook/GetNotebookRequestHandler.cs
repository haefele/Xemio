using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents.Session;
using Xemio.Logic.Database.Entities;

namespace Xemio.Logic.Requests.Notebooks.GetNotebook
{
    public class GetNotebookRequestHandler : IRequestHandler<GetNotebookRequest, Notebook>
    {
        private readonly IRequestContext _context;
        private readonly IAsyncDocumentSession _session;

        public GetNotebookRequestHandler(IRequestContext context, IAsyncDocumentSession session)
        {
            Guard.NotNull(context, nameof(context));
            Guard.NotNull(session, nameof(session));

            this._context = context;
            this._session = session;
        }

        public async Task<Notebook> Handle(GetNotebookRequest request, CancellationToken cancellationToken)
        {
            var notebook = await this._session.LoadAsync<Notebook>(request.NotebookId, cancellationToken);

            if (notebook == null || notebook.UserId != this._context.CurrentUser.UserId)
                throw new NotebookDoesNotExistException(request.NotebookId);

            return notebook;
        }
    }
}