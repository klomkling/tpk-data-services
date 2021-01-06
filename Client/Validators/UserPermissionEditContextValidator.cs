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
    public class UserPermissionEditContextValidator : AbstractValidator<UserPermissionEditContext>
    {
        private readonly IApiService _apiService;

        public UserPermissionEditContextValidator(IApiService apiService)
        {
            _apiService = apiService;

            CascadeMode = CascadeMode.Stop;

            RuleFor(u => u.ClaimTypeId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(BeUniqueAsync).WithMessage("'Restrict object' already assigned");
        }

        private async Task<bool> BeUniqueAsync(UserPermissionEditContext model, int claimTypeId,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.UserId), model.UserId),
                    model.CreateValidationColumn(nameof(model.ClaimTypeId), claimTypeId)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/users/permissions/unique-validation");

            return isUnique;
        }
    }
}