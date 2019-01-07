using System.Reactive;
using System.Threading.Tasks;
using Caliburn.Micro;
using ReactiveUI;
using UwCore;
using UwCore.Application;
using Xemio.App.Windows.ApplicationModes;
using Xemio.App.Windows.Services.Auth;
using Xemio.Client;
using Xemio.Client.Data.Endpoints.Auth;

namespace Xemio.App.Windows.Views.Login
{
    public class LoginViewModel : UwCoreScreen
    {
        private readonly IAuthService _authService;
        private readonly IShell _shell;

        private string _emailAddress;
        private string _password;

        public string EmailAddress
        {
            get => this._emailAddress;
            set => this.RaiseAndSetIfChanged(ref this._emailAddress, value);
        }

        public string Password
        {
            get => this._password;
            set => this.RaiseAndSetIfChanged(ref this._password, value);
        }

        public UwCoreCommand<Unit> Login { get; }

        public LoginViewModel(IAuthService authService, IShell shell)
        {
            this._authService = authService;
            this._shell = shell;

            this.DisplayName = "Login";

            var canLogin = this.WhenAnyValue(
                f => f.EmailAddress, 
                f => f.Password, 
                (email, password) => string.IsNullOrWhiteSpace(email) == false && string.IsNullOrWhiteSpace(password));

            this.Login = UwCoreCommand.Create(canLogin, this.LoginImpl)
                .HandleExceptions()
                .ShowLoadingOverlay("Logging in");
        }

        private async Task LoginImpl()
        {
            await this._authService.Login(this.EmailAddress, this.Password);

            var mode = IoC.Get<NotebookApplicationMode>();
            mode.NotebookId = null;

            this._shell.CurrentMode = mode;
        }
    }
}