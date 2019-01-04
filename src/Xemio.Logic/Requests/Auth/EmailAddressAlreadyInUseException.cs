using System;

namespace Xemio.Logic.Requests.Auth
{
    [Serializable]
    public class EmailAddressAlreadyInUseException : XemioException
    {
        public string EmailAddress { get; }

        public EmailAddressAlreadyInUseException(string emailAddress)
        {
            this.EmailAddress = emailAddress;
        }
    }
}