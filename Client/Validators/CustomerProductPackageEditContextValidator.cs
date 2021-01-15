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
    public class CustomerProductPackageEditContextValidator : AbstractValidator<CustomerProductPackageEditContext>
    {
        private readonly IApiService _apiService;

        public CustomerProductPackageEditContextValidator(IApiService apiService)
        {
            _apiService = apiService;

            CascadeMode = CascadeMode.Stop;

            RuleFor(s => s.PackageTypeId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithName("'Package Type'")
                .MustAsync(BeUniqueUnitAsync).WithMessage("Package type must be unique");

            RuleFor(s => s.Quantity)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);

            RuleFor(s => s.NetWeight)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);

            RuleFor(s => s.GrossWeight)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);

            RuleFor(s => s.PackagePerLayerOnPallet)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0, 999);

            RuleFor(s => s.MaximumLayerOnPallet)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0, 999);

            RuleFor(s => s.Remark)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);
        }

        private async Task<bool> BeUniqueUnitAsync(CustomerProductPackageEditContext model, int packageTypeId,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.CustomerProductId), model.CustomerProductId),
                    model.CreateValidationColumn(nameof(model.PackageTypeId), packageTypeId)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/customer-product-packages/unique-validation");

            return isUnique;
        }
    }
}