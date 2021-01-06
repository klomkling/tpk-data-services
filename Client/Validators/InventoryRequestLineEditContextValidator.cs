using FluentValidation;
using Tpk.DataServices.Shared.Data.EditContexts;

namespace Tpk.DataServices.Client.Validators
{
    public class InventoryRequestLineEditContextValidator : AbstractValidator<InventoryRequestLineEditContext>
    {
        public InventoryRequestLineEditContextValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(o => o.InventoryRequestId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();

            RuleFor(o => o.SupplierProductId)
                .NotEmpty()
                .When(o => Equals(o.ProductId, null));

            RuleFor(o => o.ProductId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .When(o => Equals(o.SupplierProductId, null));

            RuleFor(o => o.Quantity)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 999999.9999m);

            RuleFor(o => o.ReadyQuantity)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(0m)
                .LessThanOrEqualTo(o => o.Quantity);

            RuleFor(o => o.CompletedDate)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(o => o.RequestDate)
                .When(o => Equals(o.CompletedDate, null) == false);
        }
    }
}