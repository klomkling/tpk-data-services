using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tpk.DataServices.Server.Classes.Impl;
using Tpk.DataServices.Server.Services;
using Tpk.DataServices.Shared.Data.Constants;
using Tpk.DataServices.Shared.Data.Models;
using Tpk.DataServices.Shared.Data.Views;

namespace Tpk.DataServices.Server.Controllers
{
    [AuthorizeRequired]
    [ApiController]
    [Route("api/product-packages")]
    public class ProductPackagesController : TgControllerBase<ProductPackageWithDetail, ProductPackage>
    {
        private readonly IProductPackageService _productPackageService;

        public ProductPackagesController(IServiceProvider serviceProvider,
            IProductPackageService productPackageService, ILogger<ProductPackagesController> logger)
            : base(serviceProvider, productPackageService, logger)
        {
            _productPackageService = productPackageService;
        }

        [HttpGet("search")]
        public override async Task<IActionResult> SearchAsync([FromQuery] string[] columns,
            [FromQuery] string[] searchStrings, [FromQuery] string[] orderColumns = null)
        {
            return await base.SearchAsync(columns, searchStrings, orderColumns);
        }

        [HttpGet]
        public override async Task<IActionResult> GetAllAsync(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var collection =
                    await _productPackageService.GetAllAsync<ProductPackageWithDetail>(IsAdmin == false, null);
                if (_productPackageService.IsError)
                    return await ErrorResponse(_productPackageService.Exception.Message);

                var result = await Task.Run(() => DataSourceLoader.Load(collection.AsQueryable(), loadOptions));
                return Ok(result);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpGet("{id:int}")]
        public override async Task<IActionResult> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        [HttpPost]
        [ClaimRequired(RestrictItems.ProductPackages, RestrictActions.Create)]
        public override async Task<IActionResult> InsertAsync([FromBody] ProductPackage model)
        {
            return await base.InsertAsync(model);
        }

        [HttpPatch]
        [ClaimRequired(RestrictItems.ProductPackages, RestrictActions.Update)]
        public override async Task<IActionResult> UpdateAsync([FromBody] ProductPackage model)
        {
            return await base.UpdateAsync(model);
        }

        [HttpDelete]
        [ClaimRequired(RestrictItems.ProductPackages, RestrictActions.Delete)]
        public override async Task<IActionResult> DeleteAsync([FromBody] int id)
        {
            return await base.DeleteAsync(id);
        }

        [HttpDelete("bulk-delete")]
        [ClaimRequired(RestrictItems.ProductPackages, RestrictActions.Delete)]
        public override async Task<IActionResult> BulkDeleteAsync([FromBody] IReadOnlyList<int> collection)
        {
            return await base.BulkDeleteAsync(collection);
        }

        [HttpPut]
        [AuthorizeRequired(RestrictRoles.Administrator, RestrictRoles.Director)]
        public override async Task<IActionResult> RestoreAsync([FromBody] int id)
        {
            return await base.RestoreAsync(id);
        }

        [HttpPut("bulk-restore")]
        [AuthorizeRequired(RestrictRoles.Administrator, RestrictRoles.Director)]
        public override async Task<IActionResult> BulkRestoreAsync([FromBody] IReadOnlyList<int> collection)
        {
            return await base.BulkRestoreAsync(collection);
        }

        [HttpPost("unique-validation")]
        public override async Task<IActionResult> IsUniqueAsync([FromBody] ValidationRequest model)
        {
            return await base.IsUniqueAsync(model);
        }
    }
}