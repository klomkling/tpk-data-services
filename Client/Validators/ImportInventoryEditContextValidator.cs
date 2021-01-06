using FluentValidation;
using Tpk.DataServices.Shared.Data.EditContexts;

namespace Tpk.DataServices.Client.Validators
{
    public class ImportInventoryEditContextValidator : AbstractValidator<ImportInventoryEditContext>
    {
        public ImportInventoryEditContextValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(i => i.StockLocationId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithName("Stock Location")
                .GreaterThan(0).WithMessage("Stock Location is required");

            RuleFor(i => i.PackageTypeId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithName("Package Type")
                .GreaterThan(0).WithMessage("Package Type is required");

            RuleFor(i => i.PalletNo)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(30);

            RuleFor(i => i.ImportDetails)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Import Data is required");
        }
    }
}