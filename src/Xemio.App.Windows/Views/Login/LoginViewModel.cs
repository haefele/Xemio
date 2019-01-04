using System.Reactive;
using System.Threading.Tasks;
using UwCore;
using Xemio.Client;
using Xemio.Client.Data.Endpoints.Auth;

namespace Xemio.App.Windows.Views.Login
{
    public class LoginViewModel : UwCoreScreen
    {
        public UwCoreCommand<Unit> Login { get; }

        public LoginViewModel()
        {
            this.DisplayName = "Login";

            this.Login = UwCoreCommand.Create(this.LoginImpl)
                .HandleExceptions()
                .ShowLoadingOverlay("Logging in");
        }

        private async Task LoginImpl()
        {
            var client = new XemioClient();

            await client.Register(new RegisterAction
            {
                EmailAddress = "jürgen@test.de",
                Password = "12345678"
            });

            var result = await client.Login(new LoginAction
            {
                EmailAddress = "jürgen@test.de",
                Password = "12345678"
            });
        }
    }
}