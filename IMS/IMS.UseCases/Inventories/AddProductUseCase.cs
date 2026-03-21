using IMS.CoreBusiness;
using IMS.UseCases.Interfaces.Products;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Inventories;

public sealed class AddProductUseCase : IAddProductUseCase
{
    private readonly IProductRepository productRepository;

    public AddProductUseCase(IProductRepository productRepository)
    {
        ArgumentNullException.ThrowIfNull(productRepository);

        this.productRepository = productRepository;
    }

    public async Task ExecuteAsync(Product product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);

        product.Id = $"PRD-{Guid.CreateVersion7()}";

        await productRepository.AddProductAsync(product, cancellationToken).ConfigureAwait(false);
    }
}
