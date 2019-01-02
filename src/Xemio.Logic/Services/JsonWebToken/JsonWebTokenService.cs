using System;
using System.Collections.Generic;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.Extensions.Options;
using Xemio.Logic.Configuration;
using Xemio.Logic.Extensions;

namespace Xemio.Logic.Services.JsonWebToken
{
    public class JsonWebTokenService : IJsonWebTokenService
    {
        private readonly IOptionsMonitor<CryptoConfiguration> _cryptoConfiguration;

        public JsonWebTokenService(IOptionsMonitor<CryptoConfiguration> cryptoConfiguration)
        {
            Guard.NotNull(cryptoConfiguration, nameof(cryptoConfiguration));

            this._cryptoConfiguration = cryptoConfiguration;
        }

        public static class AuthTokenClaims
        {
            public static readonly string UserId = "userId";
        }
        public AuthToken GenerateAuthToken(string userId)
        {
            Guard.NotNullOrWhiteSpace(userId, nameof(userId));

            var data = new Dictionary<string, object>
            {
                [AuthTokenClaims.UserId] = userId.TrimCollectionNameFromId(),
            };

            string token = this.GenerateToken(data, this._cryptoConfiguration.CurrentValue.AuthTokenSecret);
            return new AuthToken(token);
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
