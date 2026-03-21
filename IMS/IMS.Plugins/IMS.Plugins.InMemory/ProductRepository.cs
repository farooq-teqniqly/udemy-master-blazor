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

        public Task DeleteProductAsync(string id, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(id);

            if (
                !_products.TryGetValue(
                    new Product
                    {
                        Id = id,
                        Name = string.Empty,
                        Price = 0,
                        Quantity = 0,
                    },
                    out var inventory
                )
            )
            {
                throw new InvalidOperationException($"Product with id {id} does not exist.");
            }

            _products.Remove(inventory);
            return Task.CompletedTask;
        }

        public Task<Product?> GetProductByIdAsync(string id, CancellationToken cancellationToken)
        {
            if (
                !_products.TryGetValue(
                    new Product
                    {
                        Id = id,
                        Name = string.Empty,
                        Price = 0,
                        Quantity = 0,
                    },
                    out var product
                )
            )
            {
                return Task.FromResult(null as Product);
            }

            return Task.FromResult(product)!;
        }

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

        public Task UpdateProductAsync(Product product, CancellationToken cancellationToken)
        {
            if (!_products.Contains(product))
            {
                throw new InvalidOperationException(
                    $"Product with id {product.Id} does not exist."
                );
            }

            if (
                _products.Any(i =>
                    i.Name.Equals(product.Name, StringComparison.OrdinalIgnoreCase)
                    && i.Id != product.Id
                )
            )
            {
                throw new InvalidOperationException(
                    $"Product with name {product.Name} already exists."
                );
            }

            _products.Remove(product);
            _products.Add(product);

            return Task.CompletedTask;
        }
    }
}
