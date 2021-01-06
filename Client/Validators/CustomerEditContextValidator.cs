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
    public class CustomerEditContextValidator : AbstractValidator<CustomerEditContext>
    {
        private readonly IApiService _apiService;

        public CustomerEditContextValidator(IApiService apiService)
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
                .MustAsync(BeUniqueName).WithMessage("'Name and branch' must be unique");

            RuleFor(s => s.Branch)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50)
                .MustAsync(BeUniqueBranch).WithMessage("'Name and branch' must be unique");
            
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
                .Matches("^[0-9]*$").WithMessage("Postal code must be number only")
                .MaximumLength(5);

            RuleFor(s => s.TaxId)
                .Cascade(CascadeMode.Stop)
                .Matches("^[0-9]*$").WithMessage("Tax Id must be number only")
                .MaximumLength(13);

            RuleFor(s => s.Comment)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);
        }

        private async Task<bool> BeUniqueBranch(CustomerEditContext model, string branch, CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.Name), model.Name),
                    model.CreateValidationColumn(nameof(model.Branch), branch)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/customers/unique-validation");

            return isUnique;
        }

        private async Task<bool> BeUniqueName(CustomerEditContext model, string name,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.Name), name),
                    model.CreateValidationColumn(nameof(model.Branch), model.Branch)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/customers/unique-validation");

            return isUnique;
        }

        private async Task<bool> BeUniqueCode(CustomerEditContext model, string code,
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

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/customers/unique-validation");

            return isUnique;
        }
    }
}