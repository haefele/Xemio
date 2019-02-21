using MediatR;
using Xemio.Logic.Requests;
using Xemio.Logic.Database.Entities;
using FluentValidation;
using Xemio.Logic.Extensions;

namespace Xemio.Logic.Requests.Organizations.CreateOrganization
{
    [AuthorizedRequest]
    public class CreateOrganizationRequest : IRequest<Organization>
    {
        public string Name { get; set; }
    }

    public class CreateOrganizationRequestValidator : AbstractValidator<CreateOrganizationRequest> 
    {
        public CreateOrganizationRequestValidator()
        {
            this.RuleFor(f => f.Name).NotEmpty().NoSurroundingWhitespace();
        }
    }
}