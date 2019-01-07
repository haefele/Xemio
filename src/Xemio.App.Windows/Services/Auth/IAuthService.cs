using System.Threading.Tasks;

namespace Xemio.App.Windows.Services.Auth
{
    public interface IAuthService
    {
        string Token { get; }

        Task Login(string emailAddress, string password);
        Task Logout();
    }
}