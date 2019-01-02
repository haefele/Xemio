using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Operations.CompareExchange;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using Xemio.Logic.Database.Entities;
using Xemio.Logic.Services.JsonWebToken;

namespace Xemio.Logic.Requests.Auth.LoginUser
{
    public class LoginUserRequestHandler : IRequestHandler<LoginUserRequest, AuthToken>
    {
        private readonly IJsonWebTokenService _jsonWebTokenService;
        private readonly IAsyncDocumentSession _session;

        public LoginUserRequestHandler(IJsonWebTokenService jsonWebTokenService, IAsyncDocumentSession session)
        {
            Guard.NotNull(jsonWebTokenService, nameof(jsonWebTokenService));
            Guard.NotNull(session, nameof(session));

            this._jsonWebTokenService = jsonWebTokenService;
            this._session = session;
        }

        public async Task<AuthToken> Handle(LoginUserRequest request, CancellationToken cancellationToken)
        {
            var user = await this._session.Query<User>()
                .Where(f => f.Id == RavenQuery.CmpXchg<string>($"users/{request.EmailAddress}"))
                .FirstOrDefaultAsync(cancellationToken);

            if (BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user?.PasswordHash) == false)
                throw new IncorrectPasswordException();

            return this._jsonWebTokenService.GenerateAuthToken(user?.Id);
        }
    }
}