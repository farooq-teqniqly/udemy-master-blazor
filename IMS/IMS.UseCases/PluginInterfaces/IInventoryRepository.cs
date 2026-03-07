using IMS.CoreBusiness;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IInventoryRepository
    {
        Task<IReadOnlyCollection<Inventory>> GetInventoriesByNameAsync(
            string? name,
            CancellationToken cancellationToken
        );
    }
}
