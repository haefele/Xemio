using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Raven.Client.Documents.Session;
using Xemio.Logic.Database.Entities;
using Xemio.Logic.Extensions;

namespace Xemio.Logic.Requests.Organizations
{
    [AuthorizedRequest]
    public class DeleteOrganizationRequest : IRequest
    {
        public string OrganizationId { get; set; }
    }

    public class DeleteOrganizationRequestValidator : AbstractValidator<DeleteOrganizationRequest>
    {
        public DeleteOrganizationRequestValidator()
        {
            this.RuleFor(f => f.OrganizationId).NotEmpty().NoSurroundingWhitespace();
        }
    }

    public class DeleteOrganizationRequestHandler : AsyncRequestHandler<DeleteOrganizationRequest>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IRequestContext _context;

        public DeleteOrganizationRequestHandler(IAsyncDocumentSession session, IRequestContext context)
        {
            Guard.NotNull(session, nameof(session));
            Guard.NotNull(context, nameof(context));

            this._session = session;
            this._context = context;
        }

        protected override async Task Handle(DeleteOrganizationRequest request, CancellationToken cancellationToken)
        {
            var organization = await this._session.LoadAsync<Organization>(request.OrganizationId, cancellationToken);
            
            if (organization == null || organization.Members.All(f => f.UserId != this._context.CurrentUser.UserId))
                throw new OrganizationDoesNotExistException(request.OrganizationId);

            this._session.Advanced.WaitForIndexesAfterSaveChanges();            
            this._session.Delete(organization);
        }
    }
}