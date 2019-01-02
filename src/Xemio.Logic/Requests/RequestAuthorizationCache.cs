using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Xemio.Logic.Requests
{
    public static class RequestAuthorizationCache
    {
        private static readonly ConcurrentDictionary<Type, bool> _requestTypeRequiresAuthorization = new ConcurrentDictionary<Type, bool>();

        public static bool IsAuthorizationRequired(Type requestType)
        {
            return _requestTypeRequiresAuthorization.GetOrAdd(requestType, RequestRequiresAuthorization);
        }
        private static bool RequestRequiresAuthorization(Type requestType)
        {
            var authorizedRequestAttribute = requestType.GetCustomAttribute<AuthorizedRequestAttribute>();
            var nonAuthorizedRequestAttribute = requestType.GetCustomAttribute<UnauthorizedRequestAttribute>();

            if (authorizedRequestAttribute == null && nonAuthorizedRequestAttribute == null)
                throw new RequestAuthorizationAttributeMissingException(requestType);

            return authorizedRequestAttribute != null;
        }
    }
}