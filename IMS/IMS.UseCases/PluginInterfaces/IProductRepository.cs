using IMS.CoreBusiness;

namespace IMS.UseCases.PluginInterfaces;

public interface IProductRepository
{
    Task AddProductAsync(Product product, CancellationToken cancellationToken);
    Task DeleteProductAsync(string id, CancellationToken cancellationToken);
    Task<Product?> GetProductByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Product>> GetProductsByNameAsync(
        string? name,
        CancellationToken cancellationToken
    );

    Task UpdateProductAsync(Product product, CancellationToken cancellationToken);
}
