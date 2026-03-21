using IMS.CoreBusiness;

namespace IMS.UseCases.Interfaces.Products;

public interface IViewProductsByNameUseCase
{
    Task<IEnumerable<Product>> ExecuteAsync(
        string? name = null,
        CancellationToken cancellationToken = default
    );
}
