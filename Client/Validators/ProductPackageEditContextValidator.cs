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
    public class ProductPackageEditContextValidator : AbstractValidator<ProductPackageEditContext>
    {
        private readonly IApiService _apiService;

        public ProductPackageEditContextValidator(IApiService apiService)
        {
            _apiService = apiService;

            CascadeMode = CascadeMode.Stop;

            RuleFor(p => p.Remark)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);

            RuleFor(p => p.PackageTypeId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithName("Package Type")
                .GreaterThan(0)
                .MustAsync(BeUniqueAsync).WithMessage("'Package Type' must be unique");

            RuleFor(p => p.Quantity)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 999999.99m);

            RuleFor(p => p.NetWeight)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 999999.99m);

            RuleFor(p => p.GrossWeight)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 999999.99m);
        }

        private async Task<bool> BeUniqueAsync(ProductPackageEditContext model, int packagingId,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.ProductId), model.ProductId),
                    model.CreateValidationColumn(nameof(model.PackageTypeId), packagingId)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/product-packages/unique-validation");

            return isUnique;
        }
    }
}