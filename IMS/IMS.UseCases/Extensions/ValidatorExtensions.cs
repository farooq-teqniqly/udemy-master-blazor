using FluentValidation;
using IMS.CoreBusiness;
using IMS.UseCases.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace IMS.UseCases.Extensions
{
    public static class ValidatorExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddSingleton<IValidator<Inventory>, InventoryValidator>();
            return services;
        }
    }
}
