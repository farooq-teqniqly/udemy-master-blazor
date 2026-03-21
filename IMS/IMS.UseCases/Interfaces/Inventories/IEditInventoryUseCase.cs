using IMS.CoreBusiness;

namespace IMS.UseCases.Interfaces.Inventories;

public interface IEditInventoryUseCase
{
    Task ExecuteAsync(Inventory inventory, CancellationToken cancellationToken = default);
}
