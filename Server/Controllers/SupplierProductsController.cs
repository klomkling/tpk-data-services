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
    [Route("api/supplier-products")]
    public class SupplierProductsController : TgControllerBase<SupplierProductWithDetail, SupplierProduct>
    {
        private readonly ISupplierProductPackageService _productPackageService;

        public SupplierProductsController(IServiceProvider serviceProvider,
            ISupplierProductService supplierProductService, ISupplierProductPackageService productPackageService,
            ILogger<SupplierProductsController> logger)
            : base(serviceProvider, supplierProductService, logger)
        {
            _productPackageService = productPackageService;
        }

        [HttpGet("search")]
        public override Task<IActionResult> SearchAsync([FromQuery] string[] columns,
            [FromQuery] string[] searchStrings, [FromQuery] string[] orderColumns = null)
        {
            return base.SearchAsync(columns, searchStrings, orderColumns);
        }

        [HttpGet("{id:int}/packages")]
        public async Task<IActionResult> GetSupplierProductPackageAsync(int id,
            [FromQuery] DataSourceLoadOptions loadOptions)
        {
            try
            {
                var condition = $"{nameof(SupplierProductPackage.SupplierProductId)} = {id}";

                var collection =
                    await _productPackageService.GetAllAsync<SupplierProductPackageWithDetail>(
                        IsAdmin == false, null, condition);

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

        [HttpGet]
        public override async Task<IActionResult> GetAllAsync(DataSourceLoadOptions loadOptions)
        {
            return await base.GetAllAsync(loadOptions);
        }

        [HttpGet("{id:int}")]
        public override async Task<IActionResult> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        [HttpPost]
        [ClaimRequired(RestrictItems.SupplierProducts, RestrictActions.Create)]
        public override async Task<IActionResult> InsertAsync([FromBody] SupplierProduct model)
        {
            return await base.InsertAsync(model);
        }

        [HttpPatch]
        [ClaimRequired(RestrictItems.SupplierProducts, RestrictActions.Update)]
        public override async Task<IActionResult> UpdateAsync([FromBody] SupplierProduct model)
        {
            return await base.UpdateAsync(model);
        }

        [HttpDelete]
        [ClaimRequired(RestrictItems.SupplierProducts, RestrictActions.Delete)]
        public override async Task<IActionResult> DeleteAsync([FromBody] int id)
        {
            return await base.DeleteAsync(id);
        }

        [HttpDelete("bulk-delete")]
        [ClaimRequired(RestrictItems.SupplierProducts, RestrictActions.Delete)]
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