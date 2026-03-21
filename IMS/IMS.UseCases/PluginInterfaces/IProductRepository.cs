using IMS.CoreBusiness;

namespace IMS.UseCases.PluginInterfaces;

public interface IProductRepository
{
    Task<IReadOnlyCollection<Product>> GetProductsByNameAsync(
        string? name,
        CancellationToken cancellationToken
    );
}
