using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using Xemio.Logic.Services.JsonWebToken;

namespace Xemio.Logic.Requests
{
    public class AuthorizationRequestPreProcessor<T> : IRequestPreProcessor<T>
    {
        private readonly IRequestContext _context;
        private readonly IJsonWebTokenService _jsonWebTokenService;

        public AuthorizationRequestPreProcessor(IRequestContext context, IJsonWebTokenService jsonWebTokenService)
        {
            Guard.NotNull(context, nameof(context));
            Guard.NotNull(jsonWebTokenService, nameof(jsonWebTokenService));

            this._context = context;
            this._jsonWebTokenService = jsonWebTokenService;
        }

        public Task Process(T request, CancellationToken cancellationToken)
        {
            if (RequestAuthorizationCache.IsAuthorizationRequired(request.GetType()))
            {
                if (this._context.CurrentUser == null || this._jsonWebTokenService.ValidateAuthToken(this._context.CurrentUser) == false)
                    throw new UnauthorizedRequestException();
            }

            return Task.CompletedTask;
        }
    }
}