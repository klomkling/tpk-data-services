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
    public class StockLocationEditContextValidator : AbstractValidator<StockLocationEditContext>
    {
        private readonly IApiService _apiService;

        public StockLocationEditContextValidator(IApiService apiService)
        {
            _apiService = apiService;

            const string errorMessage = "Stock location building with shelf must be unique";

            CascadeMode = CascadeMode.Stop;

            RuleFor(s => s.Building)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(20)
                .MustAsync(BeUniqueBuildingAsync).WithMessage(errorMessage);

            RuleFor(s => s.Shelf)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(20)
                .MustAsync(BeUniqueShelfAsync).WithMessage(errorMessage);
        }

        private async Task<bool> BeUniqueShelfAsync(StockLocationEditContext model, string shelf,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.Building), model.Building),
                    model.CreateValidationColumn(nameof(model.Shelf), shelf)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/stock-locations/unique-validation");

            return isUnique;
        }

        private async Task<bool> BeUniqueBuildingAsync(StockLocationEditContext model, string building,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.Building), building),
                    model.CreateValidationColumn(nameof(model.Shelf), model.Shelf)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/stock-locations/unique-validation");

            return isUnique;
        }
    }
}