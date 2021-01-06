using System;
using FluentValidation;
using Tpk.DataServices.Shared.Data.EditContexts;

namespace Tpk.DataServices.Client.Validators
{
    public class TransportationRequestEditContextValidator : AbstractValidator<TransportationRequestEditContext>
    {
        public TransportationRequestEditContextValidator()
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
                .When(r => Equals(r.CompletedDate, null) == false);

            RuleFor(r => r.Remark)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);
        }
    }
}