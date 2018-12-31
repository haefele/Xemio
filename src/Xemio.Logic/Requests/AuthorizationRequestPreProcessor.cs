using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;

namespace Xemio.Logic.Requests
{
    public class AuthorizationRequestPreProcessor<T> : IRequestPreProcessor<T>
    {
        private static readonly ConcurrentDictionary<Type, bool> _requestTypeRequiresAuthorization = new ConcurrentDictionary<Type, bool>();

        private readonly IRequestContext _context;

        public AuthorizationRequestPreProcessor(IRequestContext context)
        {
            Guard.NotNull(context, nameof(context));

            this._context = context;
        }

        public Task Process(T request, CancellationToken cancellationToken)
        {
            if (this._context.CurrentUser == null && _requestTypeRequiresAuthorization.GetOrAdd(request.GetType(), this.RequestRequiresAuthorization))
                throw new UnauthorizedRequestException();
            
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