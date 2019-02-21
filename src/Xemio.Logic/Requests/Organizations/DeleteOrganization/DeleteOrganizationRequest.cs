using FluentValidation;
using MediatR;
using Xemio.Logic.Extensions;

namespace Xemio.Logic.Requests.Organizations.DeleteOrganization
{
    [AuthorizedRequest]
    public class DeleteOrganizationRequest : IRequest
    {
        public string OrganizationId { get; set; }
    }

    public class DeleteOrganizationRequestValidator : AbstractValidator<DeleteOrganizationRequest>
    {
        public DeleteOrganizationRequestValidator()
        {
            this.RuleFor(f => f.OrganizationId).NotEmpty().NoSurroundingWhitespace();
        }
    }
}