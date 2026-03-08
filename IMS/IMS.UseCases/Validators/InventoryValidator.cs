using FluentValidation;
using IMS.CoreBusiness;

namespace IMS.UseCases.Validators
{
    public sealed class InventoryValidator : AbstractValidator<Inventory>
    {
        public InventoryValidator()
        {
            RuleFor(i => i.Name)
                .NotEmpty()
                .MinimumLength(5)
                .WithMessage("Name cannot be shorter than 5 characters.")
                .MaximumLength(100)
                .WithMessage("Name cannot be longer than 100 characters.");

            RuleFor(i => i.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Price cannot be a negative value.")
                .LessThanOrEqualTo(1000000)
                .WithMessage("Price cannot be more than 1,000,000.");

            RuleFor(i => i.Quantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Quantity cannot be a negative value.")
                .LessThanOrEqualTo(100000)
                .WithMessage("Quantity cannot be more than 100,000.");
        }
    }
}
