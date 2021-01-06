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
    public class CustomerContactEditContextValidator : AbstractValidator<CustomerContactEditContext>
    {
        private const string ErrorMessage = "Contact type with data must be unique";
        private readonly IApiService _apiService;

        public CustomerContactEditContextValidator(IApiService apiService)
        {
            _apiService = apiService;

            CascadeMode = CascadeMode.Stop;

            RuleFor(s => s.ContactTypeId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithName("Contact Type")
                .MustAsync(BeUniqueContactTypeId).WithMessage(ErrorMessage);

            RuleFor(s => s.ContactData)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithName("Contact Data")
                .MaximumLength(100).WithName("Contact Data")
                .MustAsync(BeUniqueData).WithMessage(ErrorMessage);

            RuleFor(s => s.Remark)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(250);
        }

        private async Task<bool> BeUniqueData(CustomerContactEditContext model, string data,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.ContactData), data.Trim()),
                    model.CreateValidationColumn(nameof(model.CustomerId), model.CustomerId),
                    model.CreateValidationColumn(nameof(model.ContactTypeId), model.ContactTypeId)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/customer-contacts/unique-validation");

            return isUnique;
        }

        private async Task<bool> BeUniqueContactTypeId(CustomerContactEditContext model, int contactTypeId,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.ContactTypeId), contactTypeId),
                    model.CreateValidationColumn(nameof(model.CustomerId), model.CustomerId),
                    model.CreateValidationColumn(nameof(model.ContactData), model.ContactData?.Trim())
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/customer-contacts/unique-validation");

            return isUnique;
        }
    }
}