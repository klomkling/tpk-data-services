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
    public class SupplierEditContextValidator : AbstractValidator<SupplierEditContext>
    {
        private readonly IApiService _apiService;

        public SupplierEditContextValidator(IApiService apiService)
        {
            _apiService = apiService;

            CascadeMode = CascadeMode.Stop;

            RuleFor(s => s.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(30)
                .MustAsync(BeUniqueCode).WithMessage("'Code' must be unique");

            RuleFor(s => s.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(100)
                .MustAsync(BeUniqueName).WithMessage("'Name' must be unique");

            RuleFor(s => s.Address)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);

            RuleFor(s => s.SubDistrict)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(s => s.District)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(s => s.Province)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);

            RuleFor(s => s.PostalCode)
                .Cascade(CascadeMode.Stop)
                .Matches("^[0-9]*$").WithMessage("'Postal code' must be number only")
                .MaximumLength(5);

            RuleFor(s => s.TaxId)
                .Cascade(CascadeMode.Stop)
                .Matches("^[0-9]*$").WithMessage("'Tax Id' must be number only")
                .MaximumLength(13);
        }

        private async Task<bool> BeUniqueName(SupplierEditContext model, string name,
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

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/suppliers/unique-validation");

            return isUnique;
        }

        private async Task<bool> BeUniqueCode(SupplierEditContext model, string code,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.Code), code)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/suppliers/unique-validation");

            return isUnique;
        }
    }
    
}