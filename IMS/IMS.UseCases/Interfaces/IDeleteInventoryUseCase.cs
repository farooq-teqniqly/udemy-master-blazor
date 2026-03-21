namespace IMS.UseCases.Interfaces;

public interface IDeleteInventoryUseCase
{
    Task ExecuteAsync(string id, CancellationToken cancellationToken = default);
}