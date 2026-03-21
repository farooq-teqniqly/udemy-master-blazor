using IMS.CoreBusiness;

namespace IMS.UseCases.Interfaces.Inventories;

public interface IAddInventoryUseCase
{
    Task ExecuteAsync(Inventory inventory, CancellationToken cancellationToken = default);
}
