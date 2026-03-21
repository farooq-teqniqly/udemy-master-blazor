using IMS.CoreBusiness;

namespace IMS.UseCases.Interfaces.Products;

public interface IAddProductUseCase
{
    Task ExecuteAsync(Product product, CancellationToken cancellationToken = default);
}
