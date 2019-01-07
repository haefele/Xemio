using System.Threading.Tasks;
using UwCore.Services.ApplicationState;
using Xemio.Client;
using Xemio.Client.Data.Endpoints.Auth;

namespace Xemio.App.Windows.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly XemioClient _client;

        private readonly IApplicationStateService _applicationStateService;

        public AuthService(XemioClient client, IApplicationStateService applicationStateService)
        {
            this._client = client;
            this._applicationStateService = applicationStateService.GetStateServiceFor(typeof(AuthService));
        }

        public string Token
        {
            get => this._applicationStateService.Get<string>(nameof(this.Token), ApplicationState.Local);
            private set => this._applicationStateService.Set(nameof(this.Token), value, ApplicationState.Local);
        }

        public async Task Login(string emailAddress, string password)
        {
            var loginResult = await this._client.Login(new LoginAction
            {
                EmailAddress = emailAddress,
                Password = password
            });

            this.Token = loginResult.Token;

            await this._applicationStateService.SaveStateAsync();
        }

        public async Task Logout()
        {
            this.Token = null;

            await this._applicationStateService.SaveStateAsync();
        }
    }
}