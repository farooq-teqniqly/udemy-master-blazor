using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public sealed class ProductRepository : IProductRepository
    {
        private readonly HashSet<Product> _products =
        [
            new()
            {
                Id = "PRD-019ccafe-e012-7025-8b3e-5eb52fc6f328",
                Name = "Super Duper Mountain Bike",
                Quantity = 3,
                Price = 7500,
            },
            new()
            {
                Id = $"PRD-019ccaff-493e-7ea6-b242-adbd6ff7984e",
                Name = "Ultra-light Road Bike",
                Quantity = 1,
                Price = 11000,
            },
        ];

        public Task<IReadOnlyCollection<Product>> GetProductsByNameAsync(
            string? name,
            CancellationToken cancellationToken
        )
        {
            if (string.IsNullOrEmpty(name))
            {
                return Task.FromResult<IReadOnlyCollection<Product>>(_products.AsReadOnly());
            }

            var products = _products.Where(i =>
                i.Name.Contains(name, StringComparison.OrdinalIgnoreCase)
            );

            return Task.FromResult<IReadOnlyCollection<Product>>(products.ToList().AsReadOnly());
        }
    }
}
