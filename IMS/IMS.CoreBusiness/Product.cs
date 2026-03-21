namespace IMS.CoreBusiness
{
    public sealed class Product : IEquatable<Product>
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;
        public double Price { get; set; }

        public int Quantity { get; set; }

        public static bool operator !=(Product? left, Product? right)
        {
            return !Equals(left, right);
        }

        public static bool operator ==(Product? left, Product? right)
        {
            return Equals(left, right);
        }

        public bool Equals(Product? other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Id, other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is Product other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Id);
        }
    }
}
