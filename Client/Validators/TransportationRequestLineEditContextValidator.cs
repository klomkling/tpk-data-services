using FluentValidation;
using Tpk.DataServices.Shared.Data.EditContexts;

namespace Tpk.DataServices.Client.Validators
{
    public class TransportationRequestLineEditContextValidator : AbstractValidator<TransportationRequestLineEditContext>
    {
        public TransportationRequestLineEditContextValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(r => r.TransportationRequestId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();

            RuleFor(r => r.ProductId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();

            RuleFor(r => r.Quantity)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 999999.9999m);
        }
    }
}