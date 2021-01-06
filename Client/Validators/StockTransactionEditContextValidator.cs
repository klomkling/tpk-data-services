using System;
using FluentValidation;
using Tpk.DataServices.Shared.Data.EditContexts;

namespace Tpk.DataServices.Client.Validators
{
    public class StockTransactionEditContextValidator : AbstractValidator<StockTransactionEditContext>
    {
        public StockTransactionEditContextValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(s => s.TransactionDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .GreaterThanOrEqualTo(new DateTime(2010, 1, 1));

            RuleFor(s => s.ProductId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithName("Product");

            RuleFor(s => s.StockroomId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithName("Stockroom");

            RuleFor(s => s.StockLocationId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithName("Stock Location");

            RuleFor(s => s.LotNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(s => s.PackageNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .InclusiveBetween(1, 999999);

            RuleFor(s => s.PalletNo)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(s => s.PackingReference)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(30);

            RuleFor(s => s.Quantity)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 999999.99m);

            RuleFor(s => s.Remark)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);
        }
    }
}