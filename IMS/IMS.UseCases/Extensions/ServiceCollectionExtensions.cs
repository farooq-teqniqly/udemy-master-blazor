using FluentValidation;
using IMS.CoreBusiness;
using IMS.UseCases.Interfaces.Inventories;
using IMS.UseCases.Interfaces.Products;
using IMS.UseCases.Inventories;
using IMS.UseCases.Products;
using IMS.UseCases.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace IMS.UseCases.Extensions
{
    public static class ServiceCollectionExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddUseCases()
            {
                ArgumentNullException.ThrowIfNull(services);

                services.AddTransient<
                    IViewInventoriesByNameUseCase,
                    ViewInventoriesByNameUseCase
                >();

                services.AddTransient<IAddInventoryUseCase, AddInventoryUseCase>();
                services.AddTransient<IEditInventoryUseCase, EditInventoryUseCase>();
                services.AddTransient<IViewInventoryByIdUseCase, ViewInventoryByIdUseCase>();
                services.AddTransient<IDeleteInventoryUseCase, DeleteInventoryUseCase>();

                services.AddTransient<IAddProductUseCase, AddProductUseCase>();
                services.AddTransient<IEditProductUseCase, EditProductUseCase>();
                services.AddTransient<IViewProductsByNameUseCase, ViewProductsByNameUseCase>();
                services.AddTransient<IViewProductByIdUseCase, ViewProductByIdUseCase>();
                services.AddTransient<IDeleteProductUseCase, DeleteProductUseCase>();

                return services;
            }

            public IServiceCollection AddValidators()
            {
                ArgumentNullException.ThrowIfNull(services);

                services.AddSingleton<IValidator<Inventory>, InventoryValidator>();
                services.AddSingleton<IValidator<Product>, ProductValidator>();

                return services;
            }
        }
    }
}
