using System;

namespace Xemio.Logic.Requests.Auth
{
    [Serializable]
    public class NoUserWithEmailAddressExistsException : XemioException
    {
        public string EmailAddress { get; }

        public NoUserWithEmailAddressExistsException(string emailAddress)
        {
            this.EmailAddress = emailAddress;
        }
    }
}