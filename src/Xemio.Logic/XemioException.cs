using System;
using System.Runtime.Serialization;

namespace Xemio.Logic
{
    [Serializable]
    public abstract class XemioException : Exception
    {
        public XemioException()
        {
        }

        public XemioException(string message) 
            : base(message)
        {
        }

        public XemioException(string message, Exception inner) 
            : base(message, inner)
        {
        }

        protected XemioException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}