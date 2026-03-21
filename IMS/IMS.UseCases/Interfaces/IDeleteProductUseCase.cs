namespace IMS.UseCases.Interfaces;

public interface IDeleteProductUseCase
{
    Task ExecuteAsync(string id, CancellationToken cancellationToken = default);
}