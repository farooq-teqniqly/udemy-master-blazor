using IMS.CoreBusiness;
using IMS.UseCases.Interfaces;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Inventories
{
    public sealed class AddInventoryUseCase : IAddInventoryUseCase
    {
        private readonly IInventoryRepository _inventoryRepository;

        public AddInventoryUseCase(IInventoryRepository inventoryRepository)
        {
            ArgumentNullException.ThrowIfNull(inventoryRepository);

            _inventoryRepository = inventoryRepository;
        }

        public async Task ExecuteAsync(
            Inventory inventory,
            CancellationToken cancellationToken = default
        )
        {
            ArgumentNullException.ThrowIfNull(inventory);

            inventory.Id = $"INV-{Guid.CreateVersion7()}";

            await _inventoryRepository
                .AddInventoryAsync(inventory, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
