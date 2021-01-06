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
    public class ContactTypeEditContextValidator : AbstractValidator<ContactTypeEditContext>
    {
        private readonly IApiService _apiService;

        public ContactTypeEditContextValidator(IApiService apiService)
        {
            _apiService = apiService;

            CascadeMode = CascadeMode.Stop;

            RuleFor(c => c.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(30)
                .MustAsync(BeUniqueAsync).WithMessage("'Name' must be unique");

            RuleFor(c => c.Description)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);
        }

        private async Task<bool> BeUniqueAsync(ContactTypeEditContext model, string name,
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

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/contact-types/unique-validation");

            return isUnique;
        }
    }
}