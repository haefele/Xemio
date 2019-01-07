using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using UwCore.Application;
using UwCore.Hamburger;
using Xemio.App.Windows.Services.Auth;

namespace Xemio.App.Windows.ApplicationModes
{
    public class NotebookApplicationMode : ShellMode
    {
        private readonly IAuthService _authService;

        private readonly ClickableHamburgerItem _logoutItem;

        public string NotebookId { get; set; }

        public NotebookApplicationMode(IAuthService authService)
        {
            this._authService = authService;
            this._logoutItem = new ClickableHamburgerItem("Logout", Symbol.Cancel, this.Logout);
        }

        private async void Logout()
        {
            await this._authService.Logout();

            this.Shell.CurrentMode = IoC.Get<LoggedOutApplicationMode>();
        }

        protected override Task OnEnter()
        {
            return Task.CompletedTask;
        }

        protected override Task AddActions()
        {
            this.Shell.SecondaryActions.Add(this._logoutItem);

            return Task.CompletedTask;
        }

        protected override Task RemoveActions()
        {
            this.Shell.SecondaryActions.Remove(this._logoutItem);

            return Task.CompletedTask;
        }
    }
}