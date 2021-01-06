using FluentValidation;
using Tpk.DataServices.Shared.Data.EditContexts;

namespace Tpk.DataServices.Client.Validators
{
    public class CustomerOrderLineEditContextValidator : AbstractValidator<CustomerOrderLineEditContext>
    {
        public CustomerOrderLineEditContextValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(ol => ol.ProductId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithName("Product")
                .GreaterThan(0).WithName("Product");

            RuleFor(ol => ol.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(ol => ol.Quantity)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 99999999.9999m);

            RuleFor(ol => ol.Price)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);

            RuleFor(ol => ol.DiscountRate)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 100m);

            RuleFor(ol => ol.DiscountAmount)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(0m)
                .LessThanOrEqualTo(o => o.Amount);

            RuleFor(ol => ol.Remark)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);
        }
    }
}