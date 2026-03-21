using IMS.CoreBusiness;

namespace IMS.UseCases.Interfaces;

public interface IEditProductUseCase
{
    Task ExecuteAsync(Product product, CancellationToken cancellationToken = default);
}