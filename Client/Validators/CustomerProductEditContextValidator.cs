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
    public class CustomerProductEditContextValidator : AbstractValidator<CustomerProductEditContext>
    {
        private readonly IApiService _apiService;

        public CustomerProductEditContextValidator(IApiService apiService)
        {
            _apiService = apiService;

            CascadeMode = CascadeMode.Stop;

            RuleFor(s => s.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(30)
                .MustAsync(BeUniqueCodeAsync).WithMessage("Product code must be unique");

            RuleFor(s => s.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(s => s.ProductId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithName("Product");

            RuleFor(s => s.ProductUnitId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithName("Product Unit");

            RuleFor(s => s.NormalPrice)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);

            RuleFor(s => s.MoqPrice)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);

            RuleFor(s => s.MinimumOrder)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 999999.9999m);
        }

        private async Task<bool> BeUniqueCodeAsync(CustomerProductEditContext model, string code,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.Code), code),
                    model.CreateValidationColumn(nameof(model.CustomerId), model.CustomerId)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/customer-products/unique-validation");

            return isUnique;
        }
    }
}