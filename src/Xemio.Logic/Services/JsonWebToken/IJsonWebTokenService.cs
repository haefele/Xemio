namespace Xemio.Logic.Services.JsonWebToken
{
    public interface IJsonWebTokenService
    {
        AuthToken GenerateAuthToken(string userId);
        bool ValidateAuthToken(AuthToken loginToken);
    }
}