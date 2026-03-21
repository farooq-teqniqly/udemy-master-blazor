using IMS.CoreBusiness;

namespace IMS.UseCases.Interfaces.Inventories;

public interface IViewInventoryByIdUseCase
{
    Task<Inventory?> ExecuteAsync(string id, CancellationToken cancellationToken = default);
}
