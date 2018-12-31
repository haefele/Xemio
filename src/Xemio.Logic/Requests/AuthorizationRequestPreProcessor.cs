using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using Xemio.Logic.Services.JsonWebToken;

namespace Xemio.Logic.Requests
{
    public class AuthorizationRequestPreProcessor<T> : IRequestPreProcessor<T>
    {
        private static readonly ConcurrentDictionary<Type, bool> _requestTypeRequiresAuthorization = new ConcurrentDictionary<Type, bool>();

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
            var authorizationRequired = _requestTypeRequiresAuthorization.GetOrAdd(request.GetType(), this.RequestRequiresAuthorization);

            if (authorizationRequired)
            {
                if (this._context.CurrentUser == null || this._jsonWebTokenService.ValidateAuthToken(this._context.CurrentUser) == false)
                    throw new UnauthorizedRequestException();
            }

            return Task.CompletedTask;
        }

        private bool RequestRequiresAuthorization(Type requestType)
        {
            var authorizedRequestAttribute = requestType.GetCustomAttribute<AuthorizedRequestAttribute>();
            var nonAuthorizedRequestAttribute = requestType.GetCustomAttribute<UnauthorizedRequestAttribute>();

            if (authorizedRequestAttribute == null && nonAuthorizedRequestAttribute == null)
                throw new RequestAuthorizationAttributeMissingException(requestType);

            return authorizedRequestAttribute != null;
        }
    }

    [Serializable]
    public class RequestAuthorizationAttributeMissingException : XemioException
    {
        public Type RequestType { get; }

        public RequestAuthorizationAttributeMissingException(Type requestType)
        {
            this.RequestType = requestType;
        }
    }

    [Serializable]
    public class UnauthorizedRequestException : XemioException
    {
    }
}