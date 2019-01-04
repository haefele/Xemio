using System;
using System.Collections.Generic;
using Caliburn.Micro;
using UwCore.Application;
using Xemio.App.Windows.ApplicationModes;
using Xemio.App.Windows.Views.Login;

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
            return base.GetServiceTypes();
        }

        public override ShellMode GetCurrentMode() => IoC.Get<LoggedOutApplicationMode>();
        public override string GetErrorTitle() => "Error";
        public override string GetErrorMessage() => "An error occurred.";
        public override Type GetCommonExceptionType() => typeof(Exception);
    }
}
