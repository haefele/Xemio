using System;

namespace Xemio.Logic.Requests.Auth.RegisterUser
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