using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using Xemio.Logic.Database.Entities;
using Xemio.Logic.Extensions;
using Xemio.Logic.Services.JsonWebToken;

namespace Xemio.Logic.Requests.Auth
{
    [UnauthorizedRequest]
    public class LoginUserRequest : IRequest<AuthToken>
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }

    public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
    {
        public LoginUserRequestValidator()
        {
            this.RuleFor(f => f.EmailAddress).NotEmpty().EmailAddress().NoSurroundingWhitespace();
            this.RuleFor(f => f.Password).NotEmpty(); // Do not check MinimumLength here because we might change it in the future and still want our older users to be able to login
        }
    }

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

            if (user == null)
                throw new NoUserWithEmailAddressExistsException(request.EmailAddress);

            if (BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.PasswordHash) == false)
                throw new IncorrectPasswordException();

            return this._jsonWebTokenService.GenerateAuthToken(user);
        }
    }
}