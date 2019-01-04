using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents.Session;
using Xemio.Logic.Database.Entities;
using Xemio.Logic.Requests.Notebooks.GetNotebookHierarchy;
using Xemio.Logic.Services.EntityId;

namespace Xemio.Logic.Requests.Notebooks.CreateNotebook
{
    public class CreateNotebookRequestHandler : IRequestHandler<CreateNotebookRequest, Notebook>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IRequestContext _context;
        private readonly IEntityIdManager _entityIdManager;

        public CreateNotebookRequestHandler(IAsyncDocumentSession session, IRequestContext context, IEntityIdManager entityIdManager)
        {
            Guard.NotNull(session, nameof(session));
            Guard.NotNull(context, nameof(context));
            Guard.NotNull(entityIdManager, nameof(entityIdManager));

            this._session = session;
            this._context = context;
            this._entityIdManager = entityIdManager;
        }

        public async Task<Notebook> Handle(CreateNotebookRequest request, CancellationToken cancellationToken)
        {
            string actualParentNotebookId = null;

            if (string.IsNullOrWhiteSpace(request.ParentNotebookId) == false)
            {
                var parentNotebook = await this._session.LoadAsync<Notebook>(request.ParentNotebookId, cancellationToken);
                if (parentNotebook != null && parentNotebook.UserId == this._context.CurrentUser.UserId)
                    actualParentNotebookId = parentNotebook.Id;
            }

            var notebook = new Notebook
            {
                Id = this._entityIdManager.GenerateNew<Notebook>(),
                ParentNotebookId = actualParentNotebookId,
                UserId = this._context.CurrentUser.UserId,
                Name = request.Name
            };

            await this._session.StoreAsync(notebook, cancellationToken);

            var hierarchy = await this._context.Send(new GetNotebookHierarchyRequest(), cancellationToken);
            hierarchy.AddNewNotebook(notebook);

            return notebook;
        }
    }
}