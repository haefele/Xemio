using System;

namespace Xemio.Logic.Services.JsonWebToken
{
    [Serializable]
    public class InvalidJsonWebTokenException : XemioException
    {
        public string Token { get; }

        public InvalidJsonWebTokenException(string token, Exception innerException)
            : base(null, innerException)
        {
            this.Token = token;
        }
    }
}