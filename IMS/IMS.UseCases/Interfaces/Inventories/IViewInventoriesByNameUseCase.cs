using IMS.CoreBusiness;

namespace IMS.UseCases.Interfaces.Inventories;

public interface IViewInventoriesByNameUseCase
{
    Task<IEnumerable<Inventory>> ExecuteAsync(
        string? name = null,
        CancellationToken cancellationToken = default
    );
}
