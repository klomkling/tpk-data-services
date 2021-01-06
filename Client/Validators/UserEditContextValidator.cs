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
    public class UserEditContextValidator : AbstractValidator<UserEditContext>
    {
        private readonly IApiService _apiService;

        public UserEditContextValidator(IApiService apiService)
        {
            _apiService = apiService;

            CascadeMode = CascadeMode.Stop;

            RuleFor(u => u.Username)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Length(4, 50)
                .MustAsync(BeUniqueUsernameAsync).WithMessage("'Username' already exists");

            RuleFor(u => u.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(100)
                .MustAsync(BeUniqueEmailAsync).WithMessage("'Email' already exists");

            RuleFor(u => u.PasswordHash)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50);
        }

        private async Task<bool> BeUniqueEmailAsync(UserEditContext model, string email,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.Email), email)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/users/unique-validation");

            return isUnique;
        }

        private async Task<bool> BeUniqueUsernameAsync(UserEditContext model, string username,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.Username), username)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/users/unique-validation");

            return isUnique;
        }
    }
}