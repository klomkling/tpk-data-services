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
    [Route("api/[controller]")]
    public class SuppliersController : TgControllerBase<Supplier, Supplier>
    {
        private readonly ISupplierContactService _supplierContactService;
        private readonly ISupplierOrderService _supplierOrderService;
        private readonly ISupplierProductService _supplierProductService;

        public SuppliersController(IServiceProvider serviceProvider, ISupplierService supplierService,
            ISupplierContactService supplierContactService, ISupplierProductService supplierProductService,
            ISupplierOrderService supplierOrderService,
            ILogger<SuppliersController> logger)
            : base(serviceProvider, supplierService, logger)
        {
            _supplierContactService = supplierContactService;
            _supplierProductService = supplierProductService;
            _supplierOrderService = supplierOrderService;
        }

        [HttpGet("search")]
        public override Task<IActionResult> SearchAsync([FromQuery] string[] columns,
            [FromQuery] string[] searchStrings, [FromQuery] string[] orderColumns = null)
        {
            return base.SearchAsync(columns, searchStrings, orderColumns);
        }

        [HttpGet("{id:int}/contacts")]
        public async Task<IActionResult> GetSupplierContactsAsync(int id, [FromQuery] DataSourceLoadOptions loadOptions)
        {
            try
            {
                var condition = $"{nameof(SupplierContactWithDetail.SupplierId)} = {id}";

                var collection =
                    await _supplierContactService.GetAllAsync<SupplierContactWithDetail>(IsAdmin == false,
                        null, condition);

                if (_supplierContactService.IsError)
                    return await ErrorResponse(_supplierContactService.Exception.Message);

                var result = await Task.Run(() => DataSourceLoader.Load(collection.AsQueryable(), loadOptions));
                return Ok(result);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpGet("{id:int}/products")]
        public async Task<IActionResult> GetSupplierProductsAsync(int id, [FromQuery] DataSourceLoadOptions loadOptions)
        {
            try
            {
                var condition = $"{nameof(SupplierProductWithDetail.SupplierId)} = {id}";

                var collection =
                    await _supplierProductService.GetAllAsync<SupplierProductWithDetail>(IsAdmin == false, null,
                        condition);

                if (_supplierContactService.IsError != false)
                    return await ErrorResponse(_supplierProductService.Exception.Message);
                var result = await Task.Run(() => DataSourceLoader.Load(collection.AsQueryable(), loadOptions));
                return Ok(result);

            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpGet("{id:int}/orders")]
        public async Task<IActionResult> GetSupplierOrdersAsync(int id, [FromQuery] DataSourceLoadOptions loadOptions)
        {
            try
            {
                var condition = $"{nameof(SupplierOrderWithDetail.SupplierId)} = ${id}";

                var collection =
                    await _supplierOrderService.GetAllAsync<SupplierOrderWithDetail>(IsAdmin == false,
                        $"{nameof(SupplierOrderWithDetail.OrderNumber)} DESC", condition);

                if (_supplierOrderService.IsError)
                    return await ErrorResponse(_supplierOrderService.Exception.Message);

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
        [ClaimRequired(RestrictItems.Suppliers, RestrictActions.Create)]
        public override async Task<IActionResult> InsertAsync([FromBody] Supplier model)
        {
            return await base.InsertAsync(model);
        }

        [HttpPatch]
        [ClaimRequired(RestrictItems.Suppliers, RestrictActions.Update)]
        public override async Task<IActionResult> UpdateAsync([FromBody] Supplier model)
        {
            return await base.UpdateAsync(model);
        }

        [HttpDelete]
        [ClaimRequired(RestrictItems.Suppliers, RestrictActions.Delete)]
        public override async Task<IActionResult> DeleteAsync([FromBody] int id)
        {
            return await base.DeleteAsync(id);
        }

        [HttpDelete("bulk-delete")]
        [ClaimRequired(RestrictItems.Suppliers, RestrictActions.Delete)]
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