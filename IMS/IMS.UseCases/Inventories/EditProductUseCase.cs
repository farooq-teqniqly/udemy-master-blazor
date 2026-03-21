using IMS.CoreBusiness;
using IMS.UseCases.Interfaces;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Inventories;

public sealed class EditProductUseCase : IEditProductUseCase
{
    private readonly IProductRepository _productRepository;

    public EditProductUseCase(IProductRepository productRepository)
    {
        ArgumentNullException.ThrowIfNull(productRepository);

        _productRepository = productRepository;
    }

    public async Task ExecuteAsync(
        Product product,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(product);

        await _productRepository.UpdateProductAsync(product, cancellationToken);
    }
}