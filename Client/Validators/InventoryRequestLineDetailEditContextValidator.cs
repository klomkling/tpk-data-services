using FluentValidation;
using Tpk.DataServices.Shared.Data.EditContexts;

namespace Tpk.DataServices.Client.Validators
{
    public class InventoryRequestLineDetailEditContextValidator 
        : AbstractValidator<InventoryRequestLineDetailEditContext>
    {
        public InventoryRequestLineDetailEditContextValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(d => d.InventoryRequestLineId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();

            RuleFor(d => d.ProductId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();

            RuleFor(d => d.StockroomId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();

            RuleFor(d => d.StockLocationId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();

            RuleFor(d => d.PackageTypeId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();

            RuleFor(d => d.LotNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(d => d.PackageNumber)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(1, 999999);

            RuleFor(d => d.PalletNo)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(30);

            RuleFor(d => d.Quantity)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 999999.9999m);
        }
    }
}