using IMS.UseCases.Interfaces;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Inventories;

public sealed class DeleteInventoryUseCase : IDeleteInventoryUseCase
{
    private readonly IInventoryRepository _inventoryRepository;

    public DeleteInventoryUseCase(IInventoryRepository inventoryRepository)
    {
        ArgumentNullException.ThrowIfNull(inventoryRepository);
        _inventoryRepository = inventoryRepository;
    }

    public async Task ExecuteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        await _inventoryRepository
            .DeleteInventoryAsync(id, cancellationToken)
            .ConfigureAwait(false);
    }
}
