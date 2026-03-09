using IMS.CoreBusiness;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IInventoryRepository
    {
        Task AddInventoryAsync(Inventory inventory, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<Inventory>> GetInventoriesByNameAsync(
            string? name,
            CancellationToken cancellationToken
        );
        Task<Inventory?> GetInventoryByIdAsync(string id, CancellationToken cancellationToken);
        Task UpdateInventoryAsync(Inventory inventory, CancellationToken cancellationToken);
    }
}
