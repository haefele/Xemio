using System;
using System.Collections.Generic;
using Caliburn.Micro;
using UwCore.Application;
using Xemio.App.Windows.ApplicationModes;
using Xemio.App.Windows.Services.Auth;
using Xemio.App.Windows.Views.Login;
using Xemio.Client;

namespace Xemio.App.Windows
{
    public sealed partial class App
    {
        public App()
        {
            this.InitializeComponent();
        }

        public override IEnumerable<Type> GetViewModelTypes()
        {
            yield return typeof(LoginViewModel);
        }

        public override IEnumerable<Type> GetShellModeTypes()
        {
            yield return typeof(LoggedOutApplicationMode);
        }

        public override IEnumerable<Type> GetServiceTypes()
        {
            yield return typeof(XemioClient);
            yield return typeof(XemioClient);

            yield return typeof(IAuthService);
            yield return typeof(AuthService);
        }

        public override ShellMode GetCurrentMode() => IoC.Get<LoggedOutApplicationMode>();
        public override string GetErrorTitle() => "Error";
        public override string GetErrorMessage() => "An error occurred.";
        public override Type GetCommonExceptionType() => typeof(Exception);
    }
}
