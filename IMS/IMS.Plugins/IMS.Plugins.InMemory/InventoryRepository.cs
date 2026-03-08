using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public sealed class InventoryRepository : IInventoryRepository
    {
        private readonly HashSet<Inventory> _inventories =
        [
            new()
            {
                Id = $"INV-019ccafe-e012-7025-8b3e-5eb52fc6f328",
                Name = "Bike seat",
                Quantity = 10,
                Price = 80,
            },
            new()
            {
                Id = "INV-019ccaff-493e-7ea6-b242-adbd6ff7984e",
                Name = "Bike body",
                Quantity = 3,
                Price = 2500,
            },
            new()
            {
                Id = "INV-019ccaff-a10d-7a80-8ab4-f9488eb670cc",
                Name = "Bike wheels",
                Quantity = 10,
                Price = 175,
            },
            new()
            {
                Id = "INV-019ccaff-b8f4-7256-86fc-59c0b5be057f",
                Name = "Bike pedals",
                Quantity = 20,
                Price = 100,
            },
        ];

        public Task AddInventoryAsync(
            Inventory inventory,
            CancellationToken cancellationToken = default
        )
        {
            ArgumentNullException.ThrowIfNull(inventory);

            return !_inventories.Add(inventory)
                ? throw new InvalidOperationException($"Item with {inventory.Id} already exists.")
                : Task.CompletedTask;
        }

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
