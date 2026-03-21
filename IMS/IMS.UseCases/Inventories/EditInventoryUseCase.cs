using IMS.CoreBusiness;
using IMS.UseCases.Interfaces.Inventories;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Inventories;

public sealed class EditInventoryUseCase : IEditInventoryUseCase
{
    private readonly IInventoryRepository _inventoryRepository;

    public EditInventoryUseCase(IInventoryRepository inventoryRepository)
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

        await _inventoryRepository.UpdateInventoryAsync(inventory, cancellationToken);
    }
}
