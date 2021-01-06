using System;
using FluentValidation;
using Tpk.DataServices.Shared.Data.EditContexts;

namespace Tpk.DataServices.Client.Validators
{
    public class ProductionRequestEditContextValidator : AbstractValidator<ProductionRequestEditContext>
    {
        public ProductionRequestEditContextValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(r => r.RequestDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .GreaterThanOrEqualTo(new DateTime(2010, 1, 1));
            
            RuleFor(r => r.DueDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .GreaterThanOrEqualTo(r => r.RequestDate);

            RuleFor(r => r.CompletedDate)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(r => r.RequestDate)
                .When(o => Equals(o.CompletedDate, null) == false);

            RuleFor(r => r.ProductId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();

            RuleFor(r => r.Quantity)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 999999.9999m);

            RuleFor(r => r.Comment)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);
        }
    }
}