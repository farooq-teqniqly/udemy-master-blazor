namespace IMS.UseCases.Interfaces.Products;

public interface IDeleteProductUseCase
{
    Task ExecuteAsync(string id, CancellationToken cancellationToken = default);
}