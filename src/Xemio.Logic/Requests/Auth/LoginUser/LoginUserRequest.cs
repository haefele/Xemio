using FluentValidation;
using MediatR;
using Xemio.Logic.Extensions;
using Xemio.Logic.Services.JsonWebToken;

namespace Xemio.Logic.Requests.Auth.LoginUser
{
    [UnauthorizedRequest]
    public class LoginUserRequest : IRequest<AuthToken>
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }

    public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
    {
        public LoginUserRequestValidator()
        {
            this.RuleFor(f => f.EmailAddress).NotEmpty().EmailAddress().NoSurroundingWhitespace();
            this.RuleFor(f => f.Password).NotEmpty().MinimumLength(8);
        }
    }
}