using IMS.CoreBusiness;

namespace IMS.UseCases.Interfaces.Products;

public interface IViewProductByIdUseCase
{
    Task<Product?> ExecuteAsync(string id, CancellationToken cancellationToken = default);
}
