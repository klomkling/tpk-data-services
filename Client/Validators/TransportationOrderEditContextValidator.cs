using System;
using FluentValidation;
using Tpk.DataServices.Shared.Data.EditContexts;

namespace Tpk.DataServices.Client.Validators
{
    public class TransportationOrderEditContextValidator : AbstractValidator<TransportationOrderEditContext>
    {
        public TransportationOrderEditContextValidator()
        {
            CascadeMode = CascadeMode.Stop;
            
            RuleFor(r => r.OrderDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .GreaterThanOrEqualTo(new DateTime(2010, 1, 1));

            RuleFor(r => r.DueDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .GreaterThanOrEqualTo(r => r.OrderDate);

            RuleFor(r => r.CompletedDate)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(r => r.OrderDate)
                .When(r => Equals(r.CompletedDate, null) == false);

            RuleFor(r => r.TruckLicensePlate)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(r => r.DriverName)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(r => r.DriverLicenseCard)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(r => r.CoDriver1Name)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(r => r.CoDriver2Name)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(r => r.StartMileGauge)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0, 999999);

            RuleFor(r => r.EndMileGauge)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(r => r.StartMileGauge);

            RuleFor(r => r.Remark)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(200);
        }
    }
}