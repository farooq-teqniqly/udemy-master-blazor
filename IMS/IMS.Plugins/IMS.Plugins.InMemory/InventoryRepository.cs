using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public sealed class InventoryRepository : IInventoryRepository
    {
        private readonly List<Inventory> _inventories =
        [
            new()
            {
                Id = "INV-001",
                Name = "Bike seat",
                Quantity = 10,
                Price = 80,
            },
            new()
            {
                Id = "INV-002",
                Name = "Bike body",
                Quantity = 3,
                Price = 2500,
            },
            new()
            {
                Id = "INV-003",
                Name = "Bike wheels",
                Quantity = 10,
                Price = 175,
            },
            new()
            {
                Id = "INV-004",
                Name = "Bike pedals",
                Quantity = 20,
                Price = 100,
            },
        ];

        public Task<IReadOnlyCollection<Inventory>> GetInventoriesByNameAsync(
            string? name,
            CancellationToken cancellationToken
        )
        {
            if (string.IsNullOrEmpty(name))
            {
                return Task.FromResult<IReadOnlyCollection<Inventory>>(_inventories.AsReadOnly());
            }

            var inventories = _inventories.Where(i =>
                i.Name.Contains(name, StringComparison.OrdinalIgnoreCase)
            );

            return Task.FromResult<IReadOnlyCollection<Inventory>>(
                inventories.ToList().AsReadOnly()
            );
        }
    }
}
