namespace IMS.UseCases.Interfaces.Inventories;

public interface IDeleteInventoryUseCase
{
    Task ExecuteAsync(string id, CancellationToken cancellationToken = default);
}
