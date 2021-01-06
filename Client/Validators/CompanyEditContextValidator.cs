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
    public class CompanyEditContextValidator : AbstractValidator<CompanyEditContext>
    {
        private readonly IApiService _apiService;

        public CompanyEditContextValidator(IApiService apiService)
        {
            _apiService = apiService;

            CascadeMode = CascadeMode.Stop;

            RuleFor(c => c.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(100)
                .MustAsync(BeUniqueNameAsync).WithMessage("'Name' must be unique");

            RuleFor(c => c.Address)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(255);

            RuleFor(c => c.SubDistrict)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);

            RuleFor(c => c.District)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);

            RuleFor(c => c.Province)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);

            RuleFor(c => c.PostalCode)
                .Cascade(CascadeMode.Stop)
                .Matches("^[0-9]*$").WithMessage("'Postal code' must be number only")
                .MaximumLength(5);

            RuleFor(c => c.Phone)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);

            RuleFor(c => c.Fax)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);

            RuleFor(c => c.TaxId)
                .Cascade(CascadeMode.Stop)
                .Matches("^[0-9]*$").WithMessage("'Tax Id' must be number only")
                .MaximumLength(13);
        }

        private async Task<bool> BeUniqueNameAsync(CompanyEditContext model, string name,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.Name), name)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/companies/unique-validation");

            return isUnique;
        }
    }
}