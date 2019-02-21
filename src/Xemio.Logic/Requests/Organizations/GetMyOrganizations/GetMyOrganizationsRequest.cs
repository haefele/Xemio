using System.Collections.Generic;
using MediatR;
using Xemio.Logic.Database.Entities;

namespace Xemio.Logic.Requests.Organizations.GetOrganizations
{
    [AuthorizedRequest]
    public class GetMyOrganizationsRequest : IRequest<List<Organization>>
    {
    }
}