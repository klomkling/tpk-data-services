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
    public class SectionEditContextValidator : AbstractValidator<SectionEditContext>
    {
        private readonly IApiService _apiService;

        public SectionEditContextValidator(IApiService apiService)
        {
            _apiService = apiService;

            CascadeMode = CascadeMode.Stop;

            const string errorMessage = "'Name' must be unique for each department";

            RuleFor(s => s.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(100)
                .MustAsync(BeUniqueNameAsync).WithMessage(errorMessage);

            RuleFor(s => s.DepartmentId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(BeUniqueDepartmentIdAsync).WithMessage(errorMessage);
        }

        private async Task<bool> BeUniqueNameAsync(SectionEditContext model, string name,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.Name), name),
                    model.CreateValidationColumn(nameof(model.DepartmentId), model.DepartmentId)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/sections/unique-validation");

            return isUnique;
        }

        private async Task<bool> BeUniqueDepartmentIdAsync(SectionEditContext model, int departmentId,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.Name), model.Name),
                    model.CreateValidationColumn(nameof(model.DepartmentId), departmentId)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/sections/unique-validation");

            return isUnique;
        }
    }
}