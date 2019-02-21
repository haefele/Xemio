using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using Xemio.Logic.Database.Entities;
using Xemio.Logic.Database.Indexes;

namespace Xemio.Logic.Requests.Organizations
{
    [AuthorizedRequest]
    public class GetMyOrganizationsRequest : IRequest<List<Organization>>
    {
    }

    public class GetMyOrganizationsRequestHandler : IRequestHandler<GetMyOrganizationsRequest, List<Organization>>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IRequestContext _context;

        public GetMyOrganizationsRequestHandler(IAsyncDocumentSession session, IRequestContext context)
        {
            Guard.NotNull(session, nameof(session));
            Guard.NotNull(context, nameof(context));

            this._session = session;
            this._context = context;
        }

        public async Task<List<Organization>> Handle(GetMyOrganizationsRequest request, CancellationToken cancellationToken)
        {
            var organizations = await this._session.Query<Organizations_ByMemberUserIds.Result, Organizations_ByMemberUserIds>()
                .Where(f => f.UserId == this._context.CurrentUser.UserId)
                .OfType<Organization>()
                .ToListAsync(cancellationToken);

            return organizations;
        }
    }
}