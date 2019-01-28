using System;

namespace Soft.Model.Shared
{
    /// <summary>
    /// Abstract base class for an Entity.
    /// An Entity is characterized by a unique Id for all Enties of the same type.
    /// I.e. every Customer-Entity has a unique Id.
    /// </summary>
    /// <remarks>Id can be negative, when Entity is not saved yet</remarks>
    public abstract class EntityBase
    {
        #region Properties
        /// <summary>
        /// Unique Entity Id
        /// </summary>
        public virtual int Id { get; protected set; }
        #endregion

        public override bool Equals(object obj)
        {
            var other = obj as EntityBase;

            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            if (Id == 0 || other.Id == 0)
                return false;

            return Id == other.Id;
        }

        public static bool operator ==(EntityBase a, EntityBase b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(EntityBase a, EntityBase b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().ToString() + Id).GetHashCode();
        }
    }
}