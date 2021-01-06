using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
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
    [Route("api/supplier-orders")]
    public class SupplierOrdersController : TgControllerBase<SupplierOrderWithDetail, SupplierOrder>
    {
        private readonly IInventoryRequestLineService _inventoryRequestLineService;
        private readonly IInventoryRequestService _inventoryRequestService;
        private readonly ISupplierService _supplierService;
        private readonly ISupplierOrderReferenceService _supplierOrderReferenceService;
        private readonly ISupplierOrderLineReferenceService _supplierOrderLineReferenceService;
        private readonly IStockroomService _stockroomService;
        private readonly ISupplierOrderLineService _supplierOrderLineService;
        private readonly ISupplierOrderService _supplierOrderService;

        public SupplierOrdersController(IServiceProvider serviceProvider, IStockroomService stockroomService,
            ISupplierOrderService supplierOrderService, ISupplierOrderLineService supplierOrderLineService,
            IInventoryRequestService inventoryRequestService, IInventoryRequestLineService inventoryRequestLineService,
            ISupplierService supplierService, ISupplierOrderReferenceService supplierOrderReferenceService,
            ISupplierOrderLineReferenceService supplierOrderLineReferenceService,
            ILogger<SupplierOrdersController> logger)
            : base(serviceProvider, supplierOrderService, logger)
        {
            _stockroomService = stockroomService;
            _supplierOrderService = supplierOrderService;
            _supplierOrderLineService = supplierOrderLineService;
            _inventoryRequestService = inventoryRequestService;
            _inventoryRequestLineService = inventoryRequestLineService;
            _supplierService = supplierService;
            _supplierOrderReferenceService = supplierOrderReferenceService;
            _supplierOrderLineReferenceService = supplierOrderLineReferenceService;
        }

        [HttpGet("search")]
        public override Task<IActionResult> SearchAsync([FromQuery] string[] columns,
            [FromQuery] string[] searchStrings, [FromQuery] string[] orderColumns = null)
        {
            return base.SearchAsync(columns, searchStrings, orderColumns);
        }

        [HttpGet("{id:int}/lines")]
        public async Task<IActionResult> GetSupplierOrderLinesAsync(int id,
            [FromQuery] DataSourceLoadOptionsBase loadOptions)
        {
            try
            {
                var condition = $"{nameof(SupplierOrderLine.SupplierOrderId)} = {id}";

                var collection =
                    await _supplierOrderLineService.GetAllAsync<SupplierOrderLineWithDetail>(
                        IsAdmin == false, null, condition);

                if (_supplierOrderLineService.IsError)
                    return await ErrorResponse(_supplierOrderLineService.Exception.Message);

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

        [HttpGet("with-details/{id:int}")]
        public async Task<IActionResult> GetByIdWithDetailAsync(int id)
        {
            try
            {
                var result =
                    await _supplierOrderService.GetAsync<SupplierOrderWithDetail>(id, IsAdmin == false);

                if (_supplierOrderService.IsError == false)
                    return result == null ? NotFound() : (IActionResult) Ok(result);

                return await ErrorResponse(_supplierOrderService.Exception.Message);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpPost("can-generate-requests")]
        public async Task<IActionResult> CanGenerateRequests([FromBody] IReadOnlyList<int> collection)
        {
            try
            {
                var condition1 = $"{nameof(SupplierOrderWithDetail.Id)} IN ({string.Join(",", collection)})";
                var condition2 = $"{nameof(SupplierOrderWithDetail.CanGenerate)} = 1";
                var result = await _supplierOrderService.IsExistsAsync<SupplierOrderWithDetail>(IsAdmin == false,
                    condition1, condition2);
                if (_supplierOrderService.IsError) return await ErrorResponse(_supplierOrderService.Exception.Message);

                return Ok(result);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpPost("generate-request")]
        [ClaimRequired(RestrictItems.SupplierOrders, RestrictActions.Create)]
        public async Task<IActionResult> GenerateInventoryRequest([FromBody] IReadOnlyList<int> collection)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var listOfId = string.Join(",", collection.Select(id => id.ToString()));
                var condition = $"{nameof(SupplierOrderWithDetail.Id)} IN ({listOfId})";
                var result =
                    await _supplierOrderService.GetAllAsync<SupplierOrderWithDetail>(IsAdmin == false, null, condition);
                if (_supplierOrderService.IsError) return await ErrorResponse(_supplierOrderService.Exception.Message);

                var supplierOrders = result?.ToList() ?? new List<SupplierOrderWithDetail>();
                if (supplierOrders.Count == 0) return await ErrorResponse("None of supplier order is selected");

                var stockroom = await _stockroomService.GetByNameAsync("raw materials");
                if (_stockroomService.IsError) return await ErrorResponse("Cannot find stockroom");

                var generatedCount = 0;
                foreach (var supplierOrder in supplierOrders)
                {
                    if (supplierOrder.CanGenerate == false) continue;

                    condition = $"{nameof(SupplierOrderLine.SupplierOrderId)} = {supplierOrder.Id}";
                    var lineResult =
                        await _supplierOrderLineService.GetAllAsync<SupplierOrderLine>(IsAdmin == false, null,
                            condition);
                    if (_supplierOrderLineService.IsError)
                        return await ErrorResponse(_supplierOrderLineService.Exception.Message);

                    var supplierOrderLines = lineResult?.ToList() ?? new List<SupplierOrderLine>();
                    if (supplierOrderLines.Count == 0)
                        return await ErrorResponse("None of supplier order lines is available");

                    var supplierOrderReference =
                        await _supplierOrderReferenceService.GetAsync<SupplierOrderReference>(supplierOrder.Id, false);
                    if (_supplierOrderReferenceService.IsError)
                        return await ErrorResponse(_supplierOrderReferenceService.Exception.Message);

                    InventoryRequest inventoryRequest;
                    if (supplierOrderReference == null)
                    {
                        inventoryRequest = null;
                    }
                    else
                    {
                        inventoryRequest =
                            await _inventoryRequestService.GetAsync<InventoryRequest>(
                                supplierOrderReference.InventoryRequestId,
                                IsAdmin == false);
                        if (_inventoryRequestService.IsError)
                            return await ErrorResponse(_inventoryRequestService.Exception.Message);
                    }

                    var supplier = await _supplierService.GetAsync<Supplier>(supplierOrder.SupplierId);
                    if (_supplierService.IsError) return await ErrorResponse(_supplierService.Exception.Message);

                    if (inventoryRequest == null)
                    {
                        inventoryRequest = new InventoryRequest
                        {
                            RequestDate = supplierOrder.OrderDate,
                            DueDate = supplierOrder.DueDate < DateTime.Today ? DateTime.Today : supplierOrder.DueDate,
                            Status = TgOrderStatuses.New.Value,
                            StatusDate = DateTime.Today,
                            RequestType = TgInventoryRequestTypes.PurchaseOrder.Value,
                            StockroomId = stockroom.Id,
                            RequestedBy = _supplierOrderService.CurrentUsername,
                            Remark =
                                $"Purchase Order #{supplierOrder.OrderNumber:000000} : {supplier.Name}{(string.IsNullOrEmpty(supplierOrder.Comment) ? null : $", {supplierOrder.Comment}")}"
                        };

                        inventoryRequest = await _inventoryRequestService.InsertUpdateAsync(inventoryRequest);
                        if (_inventoryRequestService.IsError)
                            return await ErrorResponse(_inventoryRequestService.Exception.Message);

                        supplierOrderReference = new SupplierOrderReference
                        {
                            SupplierOrderId = supplierOrder.Id,
                            InventoryRequestId = inventoryRequest.Id
                        };

                        await _supplierOrderReferenceService.InsertUpdateAsync(supplierOrderReference);
                        if (_supplierOrderReferenceService.IsError)
                            return await ErrorResponse(_supplierOrderReferenceService.Exception.Message);
                    }

                    foreach (var orderLine in supplierOrderLines)
                    {
                        // If already generated
                        if (await _supplierOrderLineReferenceService.IsExistsAsync<SupplierOrderLineReference>(
                            IsAdmin == false,
                            $"{nameof(SupplierOrderLineReference.SupplierOrderLineId)} = {orderLine.Id}")) continue;

                        var inventoryRequestLine = new InventoryRequestLine
                        {
                            InventoryRequestId = inventoryRequest.Id,
                            SupplierProductId = orderLine?.SupplierProductId,
                            ProductId = orderLine?.ProductId,
                            Description = orderLine?.Description,
                            Quantity = orderLine?.Quantity ?? 0,
                            Status = TgOrderStatuses.New.Value,
                            StatusDate = DateTime.Today
                        };

                        inventoryRequestLine =
                            await _inventoryRequestLineService.InsertUpdateAsync(inventoryRequestLine);
                        if (_inventoryRequestLineService.IsError)
                            return await ErrorResponse(_inventoryRequestLineService.Exception.Message);

                        var supplierOrderLineReference = new SupplierOrderLineReference
                        {
                            SupplierOrderLineId = orderLine.Id,
                            InventoryRequestLineId = inventoryRequestLine.Id
                        };

                        await _supplierOrderLineReferenceService.InsertUpdateAsync(supplierOrderLineReference);
                        if (_supplierOrderLineReferenceService.IsError == false) continue;

                        return await ErrorResponse(_supplierOrderLineReferenceService.Exception.Message);
                    }

                    generatedCount++;
                }

                if (generatedCount == 0) return Ok(false);

                scope.Complete();
                return Ok(true);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpPost]
        [ClaimRequired(RestrictItems.SupplierOrders, RestrictActions.Create)]
        public override async Task<IActionResult> InsertAsync([FromBody] SupplierOrder model)
        {
            return await base.InsertAsync(model);
        }

        [HttpPatch]
        [ClaimRequired(RestrictItems.SupplierOrders, RestrictActions.Update)]
        public override async Task<IActionResult> UpdateAsync([FromBody] SupplierOrder model)
        {
            return await base.UpdateAsync(model);
        }

        [HttpDelete]
        [ClaimRequired(RestrictItems.SupplierOrders, RestrictActions.Delete)]
        public override async Task<IActionResult> DeleteAsync([FromBody] int id)
        {
            return await base.DeleteAsync(id);
        }

        [HttpDelete("bulk-delete")]
        [ClaimRequired(RestrictItems.SupplierOrders, RestrictActions.Delete)]
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