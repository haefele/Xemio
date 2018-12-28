namespace Xemio.Logic.Services.JsonWebToken
{
    public class AuthToken : JsonWebToken
    {
        public AuthToken(string token) 
            : base (token)
        {
        }

        public string UserId => this.Get<string>(JsonWebTokenService.AuthTokenClaims.UserId);
    }
}