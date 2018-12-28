using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Xemio.Logic.Requests.Auth.LoginUser
{
    public class LoginUserRequest : IRequest<string>
    {
        
    }

    public class LoginUserRequestHandler : IRequestHandler<LoginUserRequest, string>
    {
        public Task<string> Handle(LoginUserRequest request, CancellationToken cancellationToken)
        {
        }
    }
}