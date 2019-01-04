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
            
            // Do not check MinimumLength here because we might change it in the future and still want our older users to be able to login
            this.RuleFor(f => f.Password).NotEmpty(); 
        }
    }
}