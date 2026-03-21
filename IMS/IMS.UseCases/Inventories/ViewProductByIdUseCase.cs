using IMS.CoreBusiness;
using IMS.UseCases.Interfaces;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Inventories;

public sealed class ViewProductByIdUseCase : IViewProductByIdUseCase
{
    private readonly IProductRepository _productRepository;

    public ViewProductByIdUseCase(IProductRepository productRepository)
    {
        ArgumentNullException.ThrowIfNull(productRepository);

        _productRepository = productRepository;
    }

    public async Task<Product?> ExecuteAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        return await _productRepository.GetProductByIdAsync(id, cancellationToken);
    }
}
