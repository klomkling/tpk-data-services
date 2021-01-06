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
    public class SystemOptionEditContextValidator : AbstractValidator<SystemOptionEditContext>
    {
        private readonly IApiService _apiService;

        public SystemOptionEditContextValidator(IApiService apiService)
        {
            _apiService = apiService;

            CascadeMode = CascadeMode.Stop;

            RuleFor(s => s.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(50)
                .MustAsync(BeUniqueNameAsync).WithMessage("'Name' must be unique");

            RuleFor(s => s.Value)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(100);
        }

        private async Task<bool> BeUniqueNameAsync(SystemOptionEditContext model, string name,
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

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/system-options/unique-validation");

            return isUnique;
        }
    }
}