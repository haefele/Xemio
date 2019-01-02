using System;

namespace Xemio.Logic.Requests
{
    [Serializable]
    public class RequestAuthorizationAttributeMissingException : XemioException
    {
        public Type RequestType { get; }

        public RequestAuthorizationAttributeMissingException(Type requestType)
        {
            this.RequestType = requestType;
        }
    }
}