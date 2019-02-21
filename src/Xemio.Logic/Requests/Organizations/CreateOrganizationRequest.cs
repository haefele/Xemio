using MediatR;
using Xemio.Logic.Requests;
using Xemio.Logic.Database.Entities;
using FluentValidation;
using Xemio.Logic.Extensions;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents.Session;
using Xemio.Logic.Services.EntityId;

namespace Xemio.Logic.Requests.Organizations
{
    [AuthorizedRequest]
    public class CreateOrganizationRequest : IRequest<Organization>
    {
        public string Name { get; set; }
    }

    public class CreateOrganizationRequestValidator : AbstractValidator<CreateOrganizationRequest> 
    {
        public CreateOrganizationRequestValidator()
        {
            this.RuleFor(f => f.Name).NotEmpty().NoSurroundingWhitespace();
        }
    }

    public class CreateOrganizationRequestHandler : IRequestHandler<CreateOrganizationRequest, Organization>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IRequestContext _context;
        private readonly IEntityIdManager _entityIdManager;

        public CreateOrganizationRequestHandler(IAsyncDocumentSession session, IRequestContext context, IEntityIdManager entityIdManager)
        {
            Guard.NotNull(session, nameof(session));
            Guard.NotNull(context, nameof(context));
            Guard.NotNull(entityIdManager, nameof(entityIdManager));

            this._session = session;
            this._context = context;
            this._entityIdManager = entityIdManager;
        }

        public async Task<Organization> Handle(CreateOrganizationRequest request, CancellationToken cancellationToken)
        {
            var currentUser = await this._session.LoadAsync<User>(this._context.CurrentUser.UserId);

            var organization = new Organization 
            {
                Id = this._entityIdManager.GenerateNew<Organization>(),
                Name = request.Name,
                Members =
                {
                    new Member 
                    {
                        Id = this._entityIdManager.GenerateNew<Member>(),
                        UserId = this._context.CurrentUser.UserId,
                        DisplayName = currentUser.DisplayName,
                    }
                }
            };

            this._session.Advanced.WaitForIndexesAfterSaveChanges();            
            await this._session.StoreAsync(organization, cancellationToken);

            return organization;
        }
    }
}