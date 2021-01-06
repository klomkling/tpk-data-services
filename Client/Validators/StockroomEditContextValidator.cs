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
    public class StockroomEditContextValidator : AbstractValidator<StockroomEditContext>
    {
        private readonly IApiService _apiService;

        public StockroomEditContextValidator(IApiService apiService)
        {
            _apiService = apiService;

            CascadeMode = CascadeMode.Stop;

            RuleFor(s => s.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(100)
                .MustAsync(BeUniqueAsync).WithMessage("Stockroom name must be unique");

            RuleFor(m => m.Description)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(255);
        }

        private async Task<bool> BeUniqueAsync(StockroomEditContext model, string name,
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

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/stockrooms/unique-validation");

            return isUnique;
        }
    }
}