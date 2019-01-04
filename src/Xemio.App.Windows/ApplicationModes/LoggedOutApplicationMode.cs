using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using UwCore.Application;
using UwCore.Hamburger;
using Xemio.App.Windows.Views.Login;
using Xemio.App.Windows.Views.Register;

namespace Xemio.App.Windows.ApplicationModes
{
    public class LoggedOutApplicationMode : ShellMode
    {
        private readonly NavigatingHamburgerItem _loginItem;
        private readonly NavigatingHamburgerItem _registerItem;

        public LoggedOutApplicationMode()
        {
            this._loginItem = new NavigatingHamburgerItem("Login", Symbol.Accept, typeof(LoginViewModel));
            this._registerItem = new NavigatingHamburgerItem("Register", Symbol.Admin, typeof(RegisterViewModel));
        }

        protected override Task OnEnter()
        {
            this._loginItem.Execute();

            return Task.CompletedTask;
        }

        protected override Task AddActions()
        {
            this.Shell.Actions.Add(this._loginItem);
            this.Shell.Actions.Add(this._registerItem);

            return Task.CompletedTask;
        }

        protected override Task RemoveActions()
        {
            this.Shell.Actions.Remove(this._loginItem);
            this.Shell.Actions.Remove(this._registerItem);

            return Task.CompletedTask;
        }
    }
}