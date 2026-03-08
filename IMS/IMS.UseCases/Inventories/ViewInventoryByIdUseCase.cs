using IMS.CoreBusiness;
using IMS.UseCases.Interfaces;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Inventories;

public sealed class ViewInventoryByIdUseCase : IViewInventoryByIdUseCase
{
    private readonly IInventoryRepository _inventoryRepository;

    public ViewInventoryByIdUseCase(IInventoryRepository inventoryRepository)
    {
        ArgumentNullException.ThrowIfNull(inventoryRepository);

        _inventoryRepository = inventoryRepository;
    }
    public async Task<Inventory> ExecuteAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _inventoryRepository.GetInventoryByIdAsync(id, cancellationToken);
    }
}