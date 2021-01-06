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
    public class ProductEditContextValidator : AbstractValidator<ProductEditContext>
    {
        private readonly IApiService _apiService;

        public ProductEditContextValidator(IApiService apiService)
        {
            _apiService = apiService;

            CascadeMode = CascadeMode.Stop;

            RuleFor(p => p.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(30)
                .MustAsync(BeUniqueAsync).WithMessage("'Code' must be unique");

            RuleFor(p => p.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(p => p.CommonName)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);

            RuleFor(p => p.InventoryTypeId)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithName("Inventory Type");

            RuleFor(p => p.ProductUnitId)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithName("Unit");

            RuleFor(p => p.CoreWeight)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);
            
            RuleFor(p => p.StandardWeight)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);
            
            RuleFor(p => p.Width)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);

            RuleFor(p => p.Length)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);

            RuleFor(p => p.Height)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);

            RuleFor(p => p.Thickness)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);

            RuleFor(p => p.Thickness)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);

            RuleFor(p => p.NormalPrice)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);

            RuleFor(p => p.MoqPrice)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);

            RuleFor(p => p.InternalNormalPrice)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);

            RuleFor(p => p.InternalMoqPrice)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);

            RuleFor(p => p.StandardCost)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);

            RuleFor(p => p.UnitCost)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 9999.9999m);

            RuleFor(p => p.MinimumOrder)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 999999.9999m);

            RuleFor(p => p.MinimumStock)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(0m, 999999.9999m).WithName("Safety Stock");

            RuleFor(p => p.Remark)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100);
        }

        private async Task<bool> BeUniqueAsync(ProductEditContext model, string code,
            CancellationToken cancellationToken)
        {
            var req = new ValidationRequest
            {
                KeyColumn = model.CreateValidationColumn(nameof(model.Id), model.Id),
                ValidateColumns = new List<ValidationRequestColumn>
                {
                    model.CreateValidationColumn(nameof(model.Code), code)
                }
            };

            var isUnique = await _apiService.IsUniqueValueAsync(req, "api/products/unique-validation");

            return isUnique;
        }
    }
}