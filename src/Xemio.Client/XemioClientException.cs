using System;

namespace Xemio.Client
{
    public class XemioClientException : Exception
    {
        public XemioClientException(string message) 
            : base(message)
        {
        }

        public XemioClientException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}