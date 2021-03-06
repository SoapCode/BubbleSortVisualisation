using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public class BindingId : IEquatable<BindingId>
    {
        public readonly Type Type;
        public readonly string Identifier;

        public BindingId(Type type, string identifier)
        {
            Type = type;
            Identifier = identifier;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 29 + this.Type.GetHashCode();
                hash = hash * 29 + (this.Identifier == null ? 0 : this.Identifier.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object other)
        {
            if (other is BindingId)
            {
                BindingId otherId = (BindingId)other;
                return otherId == this;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(BindingId that)
        {
            return this == that;
        }

        public static bool operator ==(BindingId left, BindingId right)
        {
            return left.Type == right.Type && object.Equals(left.Identifier, right.Identifier);
        }

        public static bool operator !=(BindingId left, BindingId right)
        {
            return !left.Equals(right);
        }
    }
}
