using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Tpk.DataServices.Client.Services;
using Tpk.DataServices.Shared.Classes;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Validators
{
    public class TransportationOrderLineEditContextValidator : AbstractValidator<TransportationOrderLineEditContext>
    {
        private readonly IApiService _apiService;

        public TransportationOrderLineEditContextValidator(IApiService apiService)
        {
            _apiService = apiService;
            CascadeMode = CascadeMode.Stop;

            RuleFor(r => r.TransportationOrderId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();

            RuleFor(r => r.TransportationRequestId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(BeUniqueAsync);
        }

        private async Task<bool> BeUniqueAsync(TransportationOrderLineEditContext model, int transactionRequestId, CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.TransportationOrderId), model.TransportationOrderId),
                    model.CreateValidationColumn(nameof(model.TransportationRequestId), transactionRequestId)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/transaction-order-lines/unique-validation");

            return isUnique;
        }
    }
}