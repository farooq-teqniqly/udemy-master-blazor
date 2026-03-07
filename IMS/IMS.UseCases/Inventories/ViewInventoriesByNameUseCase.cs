using IMS.CoreBusiness;
using IMS.UseCases.Interfaces;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Inventories
{
    public sealed class ViewInventoriesByNameUseCase : IViewInventoriesByNameUseCase
    {
        private readonly IInventoryRepository _inventoryRepository;

        public ViewInventoriesByNameUseCase(IInventoryRepository inventoryRepository)
        {
            ArgumentNullException.ThrowIfNull(inventoryRepository);

            _inventoryRepository = inventoryRepository;
        }

        public async Task<IEnumerable<Inventory>> ExecuteAsync(
            string? name = null,
            CancellationToken cancellationToken = default
        )
        {
            var inventories = await _inventoryRepository
                .GetInventoriesByNameAsync(name, cancellationToken)
                .ConfigureAwait(false);

            return inventories;
        }
    }
}
