using System;

namespace Xemio.Logic.Requests.Organizations
{
    [Serializable]
    public class OrganizationDoesNotExistException : XemioException
    {
        public string OrganizationId { get; }

        public OrganizationDoesNotExistException(string organizationId)
        {
            this.OrganizationId = organizationId;
        }
    }
}