using System;
using FluentValidation;
using Tpk.DataServices.Shared.Data.EditContexts;

namespace Tpk.DataServices.Client.Validators
{
    public class InventoryRequestEditContextValidator : AbstractValidator<InventoryRequestEditContext>
    {
        public InventoryRequestEditContextValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(s => s.StockroomId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();
            
            RuleFor(s => s.RequestDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .GreaterThanOrEqualTo(new DateTime(2010, 1, 1));

            RuleFor(s => s.DueDate)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(s => s.RequestDate);

            RuleFor(o => o.CompletedDate)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(o => o.RequestDate)
                .When(o => Equals(o.CompletedDate, null) == false);

            RuleFor(o => o.RequestedBy)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(o => o.RequestApprovedBy)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(o => o.StockPerson)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(o => o.StockApprovedBy)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(o => o.AccountPerson)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(o => o.CheckedBy)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(o => o.Remark)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(200);
        }
    }
}