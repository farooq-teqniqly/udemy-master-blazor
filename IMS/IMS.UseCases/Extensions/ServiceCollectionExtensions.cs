using FluentValidation;
using IMS.CoreBusiness;
using IMS.UseCases.Interfaces;
using IMS.UseCases.Inventories;
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

                return services;
            }

            public IServiceCollection AddValidators()
            {
                ArgumentNullException.ThrowIfNull(services);

                services.AddSingleton<IValidator<Inventory>, InventoryValidator>();
                return services;
            }
        }
    }
}
