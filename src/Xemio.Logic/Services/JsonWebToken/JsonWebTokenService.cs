using System;
using System.Collections.Generic;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.Extensions.Options;
using Xemio.Logic.Configuration;
using Xemio.Logic.Database.Entities;
using Xemio.Logic.Extensions;
using Xemio.Logic.Services.IdGenerator;

namespace Xemio.Logic.Services.JsonWebToken
{
    public class JsonWebTokenService : IJsonWebTokenService
    {
        private readonly IOptionsMonitor<CryptoConfiguration> _cryptoConfiguration;
        private readonly IIdManager _idManager;

        public JsonWebTokenService(IOptionsMonitor<CryptoConfiguration> cryptoConfiguration, IIdManager idManager)
        {
            Guard.NotNull(cryptoConfiguration, nameof(cryptoConfiguration));
            Guard.NotNull(idManager, nameof(idManager));

            this._cryptoConfiguration = cryptoConfiguration;
            this._idManager = idManager;
        }

        public static class AuthTokenClaims
        {
            public static readonly string UserId = "userId";
        }
        public AuthToken GenerateAuthToken(User user)
        {
            Guard.NotNull(user, nameof(user));

            var data = new Dictionary<string, object>
            {
                [AuthTokenClaims.UserId] = this._idManager.TrimCollectionNameFromId<User>(user.Id),
            };

            string token = this.GenerateToken(data, this._cryptoConfiguration.CurrentValue.AuthTokenSecret);
            return new AuthToken(token, this._idManager);
        }
        public bool ValidateAuthToken(AuthToken loginToken)
        {
            Guard.NotNull(loginToken, nameof(loginToken));

            return this.ValidateToken(loginToken, this._cryptoConfiguration.CurrentValue.AuthTokenSecret);
        }

        private string GenerateToken(IDictionary<string, object> payload, string secret)
        {
            Guard.NotNull(payload, nameof(payload));
            Guard.NotNullOrWhiteSpace(secret, nameof(secret));

            var builder = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(secret);

            foreach (var claim in payload)
            {
                builder.AddClaim(claim.Key, claim.Value);
            }

            return builder.Build();
        }
        private bool ValidateToken(JsonWebToken token, string secret)
        {
            Guard.NotNull(token, nameof(token));
            Guard.NotNullOrWhiteSpace(secret, nameof(secret));

            try
            {
                new JwtBuilder()
                    .WithSecret(secret)
                    .MustVerifySignature()
                    .Decode(token.ToString());

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
