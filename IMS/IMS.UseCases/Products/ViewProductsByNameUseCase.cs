using IMS.CoreBusiness;
using IMS.UseCases.Interfaces;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Products
{
    public sealed class ViewProductsByNameUseCase : IViewProductsByNameUseCase
    {
        private readonly IProductRepository _productRepository;

        public ViewProductsByNameUseCase(IProductRepository productRepository)
        {
            ArgumentNullException.ThrowIfNull(productRepository);

            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> ExecuteAsync(
            string? name = null,
            CancellationToken cancellationToken = default
        )
        {
            var products = await _productRepository
                .GetProductsByNameAsync(name, cancellationToken)
                .ConfigureAwait(false);

            return products;
        }
    }
}
