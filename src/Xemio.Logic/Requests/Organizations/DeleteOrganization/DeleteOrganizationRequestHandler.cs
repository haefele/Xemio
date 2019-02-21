using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents.Session;
using Xemio.Logic.Database.Entities;

namespace Xemio.Logic.Requests.Organizations.DeleteOrganization
{
    public class DeleteOrganizationRequestHandler : AsyncRequestHandler<DeleteOrganizationRequest>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IRequestContext _context;

        public DeleteOrganizationRequestHandler(IAsyncDocumentSession session, IRequestContext context)
        {
            this._session = session;
            this._context = context;
        }

        protected override async Task Handle(DeleteOrganizationRequest request, CancellationToken cancellationToken)
        {
            var organization = await this._session.LoadAsync<Organization>(request.OrganizationId, cancellationToken);
            
            if (organization == null || organization.Members.All(f => f.UserId != this._context.CurrentUser.UserId))
                throw new OrganizationDoesNotExistException(request.OrganizationId);

            this._session.Delete(organization);
        }
    }
}