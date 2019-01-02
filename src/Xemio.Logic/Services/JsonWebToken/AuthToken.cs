using Xemio.Logic.Database.Entities;
using Xemio.Logic.Services.EntityId;

namespace Xemio.Logic.Services.JsonWebToken
{
    public class AuthToken : JsonWebToken
    {
        private readonly IEntityIdManager _entityIdManager;

        public AuthToken(string token, IEntityIdManager entityIdManager) 
            : base (token)
        {
            this._entityIdManager = entityIdManager;
        }

        public string UserId => this._entityIdManager.AddCollectionName<User>(this.Get<string>(JsonWebTokenService.AuthTokenClaims.UserId));
    }
}