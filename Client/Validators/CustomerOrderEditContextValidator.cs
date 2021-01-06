using System;
using FluentValidation;
using Tpk.DataServices.Shared.Data.EditContexts;

namespace Tpk.DataServices.Client.Validators
{
    public class CustomerOrderEditContextValidator : AbstractValidator<CustomerOrderEditContext>
    {
        public CustomerOrderEditContextValidator()
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

            RuleFor(o => o.CustomerReference)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(o => o.OurReference)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(o => o.CompletedDate)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(o => o.OrderDate)
                .When(o => Equals(o.CompletedDate, null) == false);

            RuleFor(o => o.VatRate)
                .InclusiveBetween(0m, 100m);

            RuleFor(o => o.DiscountRate)
                .InclusiveBetween(0m, 100m);

            RuleFor(o => o.DiscountAmount)
                .GreaterThanOrEqualTo(0m)
                .LessThanOrEqualTo(o => o.Amount);

            RuleFor(o => o.Comment)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(250);
        }
    }
}