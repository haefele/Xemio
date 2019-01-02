using Xemio.Logic.Database.Entities;
using Xemio.Logic.Services.IdGenerator;

namespace Xemio.Logic.Services.JsonWebToken
{
    public class AuthToken : JsonWebToken
    {
        private readonly IIdManager _idManager;

        public AuthToken(string token, IIdManager idManager) 
            : base (token)
        {
            this._idManager = idManager;
        }

        public string UserId => this._idManager.AddCollectionName<User>(this.Get<string>(JsonWebTokenService.AuthTokenClaims.UserId));
    }
}