using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents.Session;
using Xemio.Logic.Database.Entities;

namespace Xemio.Logic.Requests.Notebooks.GetNotebookHierarchy
{
    public class GetNotebookHierarchyRequestHandler : IRequestHandler<GetNotebookHierarchyRequest, NotebookHierarchy>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IRequestContext _context;

        public GetNotebookHierarchyRequestHandler(IAsyncDocumentSession session, IRequestContext context)
        {
            Guard.NotNull(session, nameof(session));
            Guard.NotNull(context, nameof(context));

            this._session = session;
            this._context = context;
        }

        public async Task<NotebookHierarchy> Handle(GetNotebookHierarchyRequest request, CancellationToken cancellationToken)
        {
            var hierarchyId = this._context.CurrentUser.UserId + "/notebookHierarchy";
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