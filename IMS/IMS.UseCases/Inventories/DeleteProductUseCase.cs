using IMS.UseCases.Interfaces.Products;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Inventories;

public sealed class DeleteProductUseCase : IDeleteProductUseCase
{
    private readonly IProductRepository _productRepository;

    public DeleteProductUseCase(IProductRepository productRepository)
    {
        ArgumentNullException.ThrowIfNull(productRepository);
        _productRepository = productRepository;
    }

    public async Task ExecuteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        await _productRepository.DeleteProductAsync(id, cancellationToken).ConfigureAwait(false);
    }
}
