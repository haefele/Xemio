using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations.CompareExchange;
using Raven.Client.Documents.Session;
using Xemio.Logic.Entities;
using Xemio.Logic.Services.IdGenerator;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Xemio.Logic.Requests.Auth.RegisterUser
{
    public class RegisterUserRequestHandler : IRequestHandler<RegisterUserRequest, User>
    {
        private readonly IDocumentStore _store;
        private readonly IAsyncDocumentSession _session;
        private readonly IIdGenerator _idGenerator;

        public RegisterUserRequestHandler(IDocumentStore store, IAsyncDocumentSession session, IIdGenerator idGenerator, IRequestContext requestContext)
        {
            Guard.NotNull(store, nameof(store));
            Guard.NotNull(session, nameof(session));
            Guard.NotNull(idGenerator, nameof(idGenerator));
            
            this._store = store;
            this._session = session;
            this._idGenerator = idGenerator;
        }

        public async Task<User> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = this._idGenerator.Generate<User>(),
                EmailAddress = request.EmailAddress,
                PasswordHash = BCryptNet.EnhancedHashPassword(request.Password),
            };

            var emailAddressOnlyOnceResult = await this._store.Operations.SendAsync(new PutCompareExchangeValueOperation<string>($"users/{request.EmailAddress}", user.Id, 0), token: cancellationToken);

            if (emailAddressOnlyOnceResult.Successful == false)
                throw new Exception("Email address already exists.");

            await this._session.StoreAsync(user, cancellationToken);

            return user;
        }
    }
}