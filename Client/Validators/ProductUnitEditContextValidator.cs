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
    public class ProductUnitEditContextValidator : AbstractValidator<ProductUnitEditContext>
    {
        private readonly IApiService _apiService;

        public ProductUnitEditContextValidator(IApiService apiService)
        {
            _apiService = apiService;

            CascadeMode = CascadeMode.Stop;

            RuleFor(p => p.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(50)
                .MustAsync(BeUniqueNameAsync).WithMessage("'Name' must be unique");

            RuleFor(p => p.Name)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);
        }

        private async Task<bool> BeUniqueNameAsync(ProductUnitEditContext model, string name,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.Code), name)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/product-units/unique-validation");

            return isUnique;
        }
    }
}