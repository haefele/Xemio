using Xemio.Logic.Database.Entities;

namespace Xemio.Logic.Services.JsonWebToken
{
    public interface IJsonWebTokenService
    {
        AuthToken GenerateAuthToken(User user);
        bool ValidateAuthToken(AuthToken loginToken);
    }
}