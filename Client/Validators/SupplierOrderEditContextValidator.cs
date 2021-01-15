using System;
using FluentValidation;
using Tpk.DataServices.Shared.Data.EditContexts;

namespace Tpk.DataServices.Client.Validators
{
    public class SupplierOrderEditContextValidator : AbstractValidator<SupplierOrderEditContext>
    {
        public SupplierOrderEditContextValidator()
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

            RuleFor(o => o.OurReference)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);
            
            RuleFor(o => o.VatRate)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 100m);

            RuleFor(o => o.DiscountRate)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 100m);

            RuleFor(o => o.DiscountAmount)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(0m)
                .LessThanOrEqualTo(o => o.Amount);

            RuleFor(o => o.Comment)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(250);
        }
    }
}