using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Raven.Client.Documents.Session;
using Xemio.Logic.Entities;
using Xemio.Logic.Requests;

namespace Xemio.Logic.Services.RegisterUser
{
    public class RegisterUserRequest : IRequest<User>
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }

    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            this.RuleFor(f => f.EmailAddress).NotEmpty().EmailAddress();
            this.RuleFor(f => f.Password).NotEmpty().MinimumLength(8);
        }
    }

    public class RegisterUserRequestHandler : IRequestHandler<RegisterUserRequest, User>
    {
        private readonly IAsyncDocumentSession _session;

        public RegisterUserRequestHandler(IAsyncDocumentSession session)
        {
            this._session = session;
        }

        public async Task<User> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            return null;
        }
    }
}