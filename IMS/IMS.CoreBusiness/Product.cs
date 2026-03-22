namespace IMS.CoreBusiness
{
    public sealed class Product : IEquatable<Product>
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;
        public double Price { get; set; }

        public List<ProductInventory> ProductInventories { get; set; } = [];
        public int Quantity { get; set; }

        public static bool operator !=(Product? left, Product? right)
        {
            return !Equals(left, right);
        }

        public static bool operator ==(Product? left, Product? right)
        {
            return Equals(left, right);
        }

        public void AddInventory(Inventory inventory)
        {
            if (
                ProductInventories.Any(pi =>
                    pi.Inventory is not null
                    && pi.Inventory.Name.Equals(inventory.Name, StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                throw new InvalidOperationException(
                    $"Inventory {inventory.Name} has already been added."
                );
            }

            ProductInventories.Add(
                new ProductInventory
                {
                    InventoryId = inventory.Id,
                    InventoryQuantity = 1,
                    Inventory = inventory,
                    ProductId = Id,
                    Product = this,
                }
            );
        }

        public bool IsValid()
        {
            return Price
                >= ProductInventories.Sum(pi => pi.Inventory?.Price * pi.InventoryQuantity);
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

        public void RemoveInventory(ProductInventory productInventory)
        {
            ProductInventories.Remove(productInventory);
        }
    }
}
