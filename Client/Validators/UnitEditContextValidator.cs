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
    public class UnitEditContextValidator : AbstractValidator<UnitEditContext>
    {
        private readonly IApiService _apiService;

        public UnitEditContextValidator(IApiService apiService)
        {
            _apiService = apiService;

            const string errorMessage = "'Name' must be unique for each section";

            CascadeMode = CascadeMode.Stop;

            RuleFor(u => u.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(100)
                .MustAsync(BeUniqueNameAsync).WithMessage(errorMessage);

            RuleFor(u => u.SectionId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(BeUniqueSectionIdAsync).WithMessage(errorMessage);
        }

        private async Task<bool> BeUniqueNameAsync(UnitEditContext model, string name,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.Name), name),
                    model.CreateValidationColumn(nameof(model.SectionId), model.SectionId)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/units/unique-validation");

            return isUnique;
        }

        private async Task<bool> BeUniqueSectionIdAsync(UnitEditContext model, int sectionId,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.Name), model.Name),
                    model.CreateValidationColumn(nameof(model.SectionId), sectionId)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/units/unique-validation");

            return isUnique;
        }
    }
}