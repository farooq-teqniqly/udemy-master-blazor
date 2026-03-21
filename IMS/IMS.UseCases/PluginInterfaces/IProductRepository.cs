using IMS.CoreBusiness;

namespace IMS.UseCases.PluginInterfaces;

public interface IProductRepository
{
    Task DeleteProductAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Product>> GetProductsByNameAsync(
        string? name,
        CancellationToken cancellationToken
    );
}
