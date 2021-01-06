using System;
using FluentValidation;
using Tpk.DataServices.Shared.Data.EditContexts;

namespace Tpk.DataServices.Client.Validators
{
    public class ProductionOrderEditContextValidator : AbstractValidator<ProductionOrderEditContext>
    {
        public ProductionOrderEditContextValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(o => o.OrderDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .GreaterThanOrEqualTo(new DateTime(2010, 1, 1));

            RuleFor(o => o.DueDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .GreaterThanOrEqualTo(o => o.OrderDate);

            RuleFor(o => o.CompletedDate)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(o => o.OrderDate)
                .When(o => Equals(o.CompletedDate, null) == false);

            RuleFor(o => o.LotNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(o => o.ProductId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();

            RuleFor(o => o.Quantity)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 999999.9999m)
                .GreaterThanOrEqualTo(o => o.DataItem.Quantity);

            RuleFor(o => o.ReadyQuantity)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .GreaterThanOrEqualTo(0m)
                .LessThanOrEqualTo(o => o.Quantity)
                .GreaterThanOrEqualTo(o => o.DataItem.ReadyQuantity);

            RuleFor(o => o.Comment)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);
        }
    }
}