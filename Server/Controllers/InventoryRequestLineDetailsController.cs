using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
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
    [Route("api/inventory-request-line-details")]
    public class
        InventoryRequestLineDetailsController : TgControllerBase<InventoryRequestLineDetailWithDetail,
            InventoryRequestLineDetail>
    {
        private readonly IInventoryRequestLineDetailService _inventoryRequestLineDetailService;

        public InventoryRequestLineDetailsController(IServiceProvider serviceProvider,
            IInventoryRequestLineDetailService inventoryRequestLineDetailService,
            ILogger<InventoryRequestLineDetailsController> logger)
            : base(serviceProvider, inventoryRequestLineDetailService, logger)
        {
            _inventoryRequestLineDetailService = inventoryRequestLineDetailService;
        }

        [HttpGet("search")]
        public override Task<IActionResult> SearchAsync([FromQuery] string[] columns,
            [FromQuery] string[] searchStrings, [FromQuery] string[] orderColumns = null)
        {
            return base.SearchAsync(columns, searchStrings, orderColumns);
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

        [HttpPost("confirm-import-receipts")]
        [ClaimRequired(RestrictItems.InventoryRequestLineDetails, RestrictActions.Create)]
        public async Task<IActionResult> ImportAsync([FromBody] IReadOnlyList<ImportInventoryLine> collection)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                if (collection == null) return await ErrorResponse("No inventory request line detail found!");

                foreach (var importInventoryLine in collection)
                {
                    if (importInventoryLine.IsValid == false) continue;

                    var lineDetail = new InventoryRequestLineDetail
                    {
                        InventoryRequestLineId = importInventoryLine.InventoryRequestLineId,
                        ProductId = importInventoryLine.ProductId,
                        StockroomId = importInventoryLine.StockroomId,
                        StockLocationId = importInventoryLine.StockLocationId,
                        PackageTypeId = importInventoryLine.PackageTypeId == 0 ? null : importInventoryLine.PackageTypeId,
                        LotNumber = importInventoryLine.LotNumber,
                        PackageNumber = importInventoryLine.PackageNumber,
                        PalletNo = importInventoryLine.PalletNo,
                        Quantity = importInventoryLine.Quantity,
                        ConfirmQuantity = importInventoryLine.Quantity,
                        IsImported = true
                    };

                    await _inventoryRequestLineDetailService.InsertUpdateAsync(lineDetail);
                    if (_inventoryRequestLineDetailService.IsError)
                        return await ErrorResponse(_inventoryRequestLineDetailService.Exception.Message);
                }

                scope.Complete();
                return Ok(true);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpPost]
        [ClaimRequired(RestrictItems.InventoryRequestLineDetails, RestrictActions.Create)]
        public override async Task<IActionResult> InsertAsync([FromBody] InventoryRequestLineDetail model)
        {
            return await base.InsertAsync(model);
        }

        [HttpPatch]
        [ClaimRequired(RestrictItems.InventoryRequestLineDetails, RestrictActions.Update)]
        public override async Task<IActionResult> UpdateAsync([FromBody] InventoryRequestLineDetail model)
        {
            return await base.UpdateAsync(model);
        }

        [HttpDelete]
        [ClaimRequired(RestrictItems.InventoryRequestLineDetails, RestrictActions.Delete)]
        public override async Task<IActionResult> DeleteAsync([FromBody] int id)
        {
            return await base.DeleteAsync(id);
        }

        [HttpDelete("bulk-delete")]
        [ClaimRequired(RestrictItems.InventoryRequestLineDetails, RestrictActions.Delete)]
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