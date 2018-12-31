using System;
using System.Collections.Generic;
using JWT.Builder;
using Newtonsoft.Json;

namespace Xemio.Logic.Services.JsonWebToken
{
    public abstract class JsonWebToken : IEquatable<JsonWebToken>
    {
        private readonly string _token;
        private readonly IDictionary<string, object> _payload;

        public JsonWebToken(string token)
        {
            Guard.NotNullOrWhiteSpace(token, nameof(token));

            this._token = token;

            try
            {
                var json = new JwtBuilder().Decode(token);
                this._payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            }
            catch (Exception e)
            {
                throw new InvalidJsonWebTokenException(token, e);
            }
        }

        protected T Get<T>(string name)
        {
            if (!this._payload.TryGetValue(name, out var value))
                return default;

            if (!(value is T t))
                return default;

            return t;
        }

        public override string ToString()
        {
            return this._token;
        }

        #region Equality
        public bool Equals(JsonWebToken other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(this._token, other._token);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return this.Equals((JsonWebToken) obj);
        }

        public override int GetHashCode()
        {
            return (this._token != null ? this._token.GetHashCode() : 0);
        }

        public static bool operator ==(JsonWebToken left, JsonWebToken right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(JsonWebToken left, JsonWebToken right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}