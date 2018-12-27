using System;
using System.Collections.Generic;
using System.Text;

namespace Xemio.Logic.Entities
{
    public abstract class AggregateRoot : IEquatable<AggregateRoot>
    {
        public string Id { get; set; }

        #region Equality
        public bool Equals(AggregateRoot other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(this.Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AggregateRoot) obj);
        }

        public override int GetHashCode()
        {
            return (this.Id != null ? this.Id.GetHashCode() : 0);
        }

        public static bool operator ==(AggregateRoot left, AggregateRoot right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AggregateRoot left, AggregateRoot right)
        {
            return !Equals(left, right);
        }
        #endregion
    }

    public class User : AggregateRoot
    {
        public string EmailAddress { get; set; }
        public bool EmailAddressIsVerified { get; set; }
        
        public string PasswordHash { get; set; }
    }
}
