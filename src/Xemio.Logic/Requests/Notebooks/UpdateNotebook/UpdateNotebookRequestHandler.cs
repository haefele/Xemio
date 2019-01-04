using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents.Session;
using Xemio.Logic.Database.Entities;
using Xemio.Logic.Extensions;
using Xemio.Logic.Requests.Notebooks.GetNotebookHierarchy;

namespace Xemio.Logic.Requests.Notebooks.UpdateNotebook
{
    public class UpdateNotebookRequestHandler : IRequestHandler<UpdateNotebookRequest, Notebook>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IRequestContext _context;

        public UpdateNotebookRequestHandler(IAsyncDocumentSession session, IRequestContext context)
        {
            Guard.NotNull(session, nameof(session));
            Guard.NotNull(context, nameof(context));

            this._session = session;
            this._context = context;
        }

        public async Task<Notebook> Handle(UpdateNotebookRequest request, CancellationToken cancellationToken)
        {
            var notebook = await this._session.LoadAsync<Notebook>(request.NotebookId, cancellationToken);
            
            if (notebook == null || notebook.UserId != this._context.CurrentUser.UserId)
                throw new NotebookDoesNotExistException(request.NotebookId);

            var hierarchy = await this._context.Send(new GetNotebookHierarchyRequest(), cancellationToken);

            if (request.UpdateName)
            {
                notebook.Name = request.Name;
            }

            if (request.UpdateParentNotebookId)
            {
                string actualParentNotebookId = null;

                if (string.IsNullOrWhiteSpace(request.ParentNotebookId) == false)
                {
                    var parentNotebook = await this._session.LoadAsync<Notebook>(request.ParentNotebookId, cancellationToken);
                    if (parentNotebook != null && parentNotebook.UserId == this._context.CurrentUser.UserId)
                    {
                        var notebookInHierarchy = hierarchy.Notebooks.Flatten(f => f.Notebooks).First(f => f.NotebookId == notebook.Id);

                        // Can't move a notebook into a children notebook of itself (avoid recursion)
                        actualParentNotebookId = notebookInHierarchy.Notebooks.Flatten(f => f.Notebooks).Any(f => f.NotebookId == parentNotebook.Id) 
                            ? null 
                            : parentNotebook.Id;
                    }
                }

                notebook.ParentNotebookId = actualParentNotebookId;
            }

            hierarchy.UpdateNotebook(notebook);

            return notebook;
        }
    }
}