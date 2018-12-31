using System;

namespace Xemio.Logic.Database.Entities
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
            return this.Equals((AggregateRoot) obj);
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
}
