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
    public class ProductLineEditContextValidator : AbstractValidator<ProductLineEditContext>
    {
        private readonly IApiService _apiService;

        public ProductLineEditContextValidator(IApiService apiService)
        {
            _apiService = apiService;

            CascadeMode = CascadeMode.Stop;

            RuleFor(p => p.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(100)
                .MustAsync(BeUniqueAsync).WithMessage("'Name' must be unique");
        }

        private async Task<bool> BeUniqueAsync(ProductLineEditContext model, string name,
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

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/product-lines/unique-validation");

            return isUnique;
        }
    }
}