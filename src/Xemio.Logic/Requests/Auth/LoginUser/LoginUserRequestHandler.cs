using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations.CompareExchange;
using Raven.Client.Documents.Session;
using Xemio.Logic.Entities;
using Xemio.Logic.Services.JsonWebToken;

namespace Xemio.Logic.Requests.Auth.LoginUser
{
    public class LoginUserRequestHandler : IRequestHandler<LoginUserRequest, AuthToken>
    {
        private readonly IJsonWebTokenService _jsonWebTokenService;
        private readonly IAsyncDocumentSession _session;
        private readonly IDocumentStore _store;

        public LoginUserRequestHandler(IJsonWebTokenService jsonWebTokenService, IAsyncDocumentSession session, IDocumentStore store)
        {
            Guard.NotNull(jsonWebTokenService, nameof(jsonWebTokenService));
            Guard.NotNull(session, nameof(session));
            Guard.NotNull(store, nameof(store));

            this._jsonWebTokenService = jsonWebTokenService;
            this._session = session;
            this._store = store;
        }

        public async Task<AuthToken> Handle(LoginUserRequest request, CancellationToken cancellationToken)
        {
            var userEmailAddress = await this._store.Operations.SendAsync(new GetCompareExchangeValueOperation<string>($"users/{request.EmailAddress}"), token:cancellationToken);

            if (userEmailAddress == null)
                throw new NoUserWithEmailAddressExistsException(request.EmailAddress);
            
            string userId = userEmailAddress.Value;

            var user = await this._session.LoadAsync<User>(userId, cancellationToken);

            if (BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user?.PasswordHash) == false)
                throw new IncorrectPasswordException();

            return this._jsonWebTokenService.GenerateAuthToken(user?.Id);
        }
    }
}