using FluentValidation;
using Tpk.DataServices.Shared.Data.EditContexts;

namespace Tpk.DataServices.Client.Validators
{
    public class CustomerAddressEditContextValidator : AbstractValidator<CustomerAddressEditContext>
    {
        public CustomerAddressEditContextValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(c => c.CustomerId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();

            RuleFor(c => c.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(c => c.Recipient)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(c => c.Address)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);

            RuleFor(c => c.SubDistrict)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(c => c.District)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(c => c.Province)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(c => c.PostalCode)
                .Cascade(CascadeMode.Stop)
                .Matches("^[0-9]*$").WithMessage("Postal code must be number only")
                .MaximumLength(5);

            RuleFor(c => c.Remark)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);
        }
    }
}