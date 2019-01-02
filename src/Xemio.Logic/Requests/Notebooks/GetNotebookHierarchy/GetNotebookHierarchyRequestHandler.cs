using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents.Session;
using Xemio.Logic.Database.Entities;
using Xemio.Logic.Services.EntityId;

namespace Xemio.Logic.Requests.Notebooks.GetNotebookHierarchy
{
    public class GetNotebookHierarchyRequestHandler : IRequestHandler<GetNotebookHierarchyRequest, NotebookHierarchy>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IRequestContext _context;
        private readonly IEntityIdManager _entityIdManager;

        public GetNotebookHierarchyRequestHandler(IAsyncDocumentSession session, IRequestContext context, IEntityIdManager entityIdManager)
        {
            Guard.NotNull(session, nameof(session));
            Guard.NotNull(context, nameof(context));
            Guard.NotNull(entityIdManager, nameof(entityIdManager));

            this._session = session;
            this._context = context;
            this._entityIdManager = entityIdManager;
        }

        public async Task<NotebookHierarchy> Handle(GetNotebookHierarchyRequest request, CancellationToken cancellationToken)
        {
            var hierarchyId = this._entityIdManager.GenerateNotebookHierarchyId(this._context.CurrentUser.UserId);
            var hierarchy = await this._session.LoadAsync<NotebookHierarchy>(hierarchyId, cancellationToken);

            if (hierarchy == null)
            {
                hierarchy = new NotebookHierarchy
                {
                    Id = hierarchyId,
                    UserId = this._context.CurrentUser.UserId,
                };

                await this._session.StoreAsync(hierarchy, cancellationToken);
            }

            return hierarchy;
        }
    }
}