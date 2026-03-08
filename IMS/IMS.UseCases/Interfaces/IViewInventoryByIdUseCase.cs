using IMS.CoreBusiness;

namespace IMS.UseCases.Interfaces;

public interface IViewInventoryByIdUseCase
{
    Task<Inventory> ExecuteAsync(string id, CancellationToken cancellationToken = default);
}