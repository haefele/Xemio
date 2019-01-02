using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations.CompareExchange;
using Raven.Client.Documents.Session;
using Xemio.Logic.Database.Entities;
using Xemio.Logic.Services.EntityId;

namespace Xemio.Logic.Requests.Auth.RegisterUser
{
    public class RegisterUserRequestHandler : IRequestHandler<RegisterUserRequest, User>
    {
        private readonly IDocumentStore _store;
        private readonly IAsyncDocumentSession _session;
        private readonly IEntityIdManager _entityIdManager;

        public RegisterUserRequestHandler(IDocumentStore store, IAsyncDocumentSession session, IEntityIdManager entityIdManager)
        {
            Guard.NotNull(store, nameof(store));
            Guard.NotNull(session, nameof(session));
            Guard.NotNull(entityIdManager, nameof(entityIdManager));
            
            this._store = store;
            this._session = session;
            this._entityIdManager = entityIdManager;
        }

        public async Task<User> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = this._entityIdManager.GenerateNew<User>(),
                EmailAddress = request.EmailAddress,
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password),
            };

            var emailAddressOnlyOnceResult = await this._store.Operations.SendAsync(new PutCompareExchangeValueOperation<string>($"users/{request.EmailAddress}", user.Id, 0), token: cancellationToken);

            if (emailAddressOnlyOnceResult.Successful == false)
                throw new EmailAddressAlreadyInUseException(request.EmailAddress);

            await this._session.StoreAsync(user, cancellationToken);

            return user;
        }
    }
}