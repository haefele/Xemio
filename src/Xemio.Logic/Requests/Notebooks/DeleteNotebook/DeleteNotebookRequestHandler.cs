using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents.Session;
using Xemio.Logic.Database.Entities;
using Xemio.Logic.Requests.Notebooks.GetNotebookHierarchy;
using Xemio.Logic.Requests.Notebooks.UpdateNotebook;

namespace Xemio.Logic.Requests.Notebooks.DeleteNotebook
{
    public class DeleteNotebookRequestHandler : AsyncRequestHandler<DeleteNotebookRequest>
    {
        private readonly IRequestContext _context;
        private readonly IAsyncDocumentSession _session;

        public DeleteNotebookRequestHandler(IRequestContext context, IAsyncDocumentSession session)
        {
            Guard.NotNull(context, nameof(context));
            Guard.NotNull(session, nameof(session));

            this._context = context;
            this._session = session;
        }
        
        protected override async Task Handle(DeleteNotebookRequest request, CancellationToken cancellationToken)
        {
            var notebook = await this._session.LoadAsync<Notebook>(request.NotebookId, cancellationToken);

            if (notebook == null || notebook.UserId != this._context.CurrentUser.UserId)
                throw new NotebookDoesNotExistException(request.NotebookId);

            var hierarchy = await this._context.Send(new GetNotebookHierarchyRequest(), cancellationToken);
            hierarchy.RemoveNotebook(notebook);

            this._session.Delete(notebook);
        }
    }
}