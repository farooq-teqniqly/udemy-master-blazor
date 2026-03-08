using System.ComponentModel.DataAnnotations;

namespace IMS.CoreBusiness
{
    public sealed class Inventory : IEquatable<Inventory>
    {
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "Inventory name is required.")]
        [MinLength(5, ErrorMessage = "Name must be at least 5 characters long.")]
        [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; } = null!;

        [Range(0, 1000000, ErrorMessage = "Price must be between 0 and 1,000,000")]
        public double Price { get; set; }

        [Range(0, 100000, ErrorMessage = "Quantity must be between 0 and 100,000.")]
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
