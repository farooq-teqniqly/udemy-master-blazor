using IMS.CoreBusiness;

namespace IMS.UseCases.Interfaces.Products;

public interface IEditProductUseCase
{
    Task ExecuteAsync(Product product, CancellationToken cancellationToken = default);
}