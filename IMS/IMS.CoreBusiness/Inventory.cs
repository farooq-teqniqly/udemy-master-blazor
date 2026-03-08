using System.ComponentModel.DataAnnotations;

namespace IMS.CoreBusiness
{
    public sealed class Inventory : IEquatable<Inventory>
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public double Price { get; set; }

        public int Quantity { get; set; }

        public static bool operator !=(Inventory? left, Inventory? right)
        {
            return !(left == right);
        }

        public static bool operator ==(Inventory? left, Inventory? right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Inventory);
        }

        public bool Equals(Inventory? other)
        {
            if (other is null)
            {
                return false;
            }

            return ReferenceEquals(this, other)
                || string.Equals(Id, other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Id);
        }
    }
}
