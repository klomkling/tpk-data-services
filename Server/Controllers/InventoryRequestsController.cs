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
using Tpk.DataServices.Shared.Classes;
using Tpk.DataServices.Shared.Data.Constants;
using Tpk.DataServices.Shared.Data.Models;
using Tpk.DataServices.Shared.Data.Views;

namespace Tpk.DataServices.Server.Controllers
{
    [AuthorizeRequired]
    [ApiController]
    [Route("api/inventory-requests")]
    public class InventoryRequestsController : TgControllerBase<InventoryRequestWithDetail, InventoryRequest>
    {
        private readonly IInventoryRequestService _inventoryRequestService;
        private readonly IInventoryRequestLineService _inventoryRequestLineService;
        private readonly IInventoryRequestLineDetailService _inventoryRequestLineDetailService;
        private readonly IStockTransactionService _stockTransactionService;
        private readonly IStockroomService _stockroomService;
        private readonly IStockBookingReferenceService _stockBookingReferenceService;
        private readonly IProductionRequestReferenceService _productionRequestReferenceService;
        private readonly IProductionInventoryReferenceService _productionInventoryReferenceService;
        private readonly IProductionOrderReferenceService _productionOrderReferenceService;
        private readonly ISupplierOrderReferenceService _supplierOrderReferenceService;
        private readonly ISupplierOrderService _supplierOrderService;
        private readonly ITransportationRequestService _transportationRequestService;
        private readonly ITransportationRequestReferenceService _transportationRequestReferenceService;
        private readonly ITransportationRequestLineService _transportationRequestLineService;
        private readonly ITransportationRequestLineReferenceService _transportationRequestLineReferenceService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerContactService _customerContactService;

        public InventoryRequestsController(IServiceProvider serviceProvider,
            IInventoryRequestService inventoryRequestService, IInventoryRequestLineService inventoryRequestLineService,
            IInventoryRequestLineDetailService inventoryRequestLineDetailService,
            IStockTransactionService stockTransactionService, IStockroomService stockroomService,
            IStockBookingReferenceService stockBookingReferenceService,
            IProductionRequestReferenceService productionRequestReferenceService,
            IProductionInventoryReferenceService productionInventoryReferenceService,
            IProductionOrderReferenceService productionOrderReferenceService,
            ISupplierOrderReferenceService supplierOrderReferenceService, ISupplierOrderService supplierOrderService,
            ITransportationRequestService transportationRequestService,
            ITransportationRequestReferenceService transportationRequestReferenceService,
            ITransportationRequestLineService transportationRequestLineService,
            ITransportationRequestLineReferenceService transportationRequestLineReferenceService,
            ICustomerService customerService, ICustomerContactService customerContactService,
            ILogger<InventoryRequestsController> logger)
            : base(serviceProvider, inventoryRequestService, logger)
        {
            _inventoryRequestService = inventoryRequestService;
            _inventoryRequestLineService = inventoryRequestLineService;
            _inventoryRequestLineDetailService = inventoryRequestLineDetailService;
            _stockTransactionService = stockTransactionService;
            _stockroomService = stockroomService;
            _stockBookingReferenceService = stockBookingReferenceService;
            _productionRequestReferenceService = productionRequestReferenceService;
            _productionInventoryReferenceService = productionInventoryReferenceService;
            _productionOrderReferenceService = productionOrderReferenceService;
            _supplierOrderReferenceService = supplierOrderReferenceService;
            _supplierOrderService = supplierOrderService;
            _transportationRequestService = transportationRequestService;
            _transportationRequestReferenceService = transportationRequestReferenceService;
            _transportationRequestLineService = transportationRequestLineService;
            _transportationRequestLineReferenceService = transportationRequestLineReferenceService;
            _customerService = customerService;
            _customerContactService = customerContactService;
        }

        [HttpGet("search")]
        public override Task<IActionResult> SearchAsync([FromQuery] string[] columns,
            [FromQuery] string[] searchStrings, [FromQuery] string[] orderColumns = null)
        {
            return base.SearchAsync(columns, searchStrings, orderColumns);
        }

        [HttpGet("{id:int}/lines")]
        public async Task<IActionResult> GetLinesAsync(int id, [FromQuery] DataSourceLoadOptions loadOptions)
        {
            try
            {
                var condition = $"{nameof(InventoryRequestLine.InventoryRequestId)} = {id}";

                var collection =
                    await _inventoryRequestLineService.GetAllAsync<InventoryRequestLineWithDetail>(
                        IsAdmin == false, null, condition);

                if (_inventoryRequestLineService.IsError)
                    return await ErrorResponse(_inventoryRequestLineService.Exception.Message);

                var result = await Task.Run(() => DataSourceLoader.Load(collection.AsQueryable(), loadOptions));
                return Ok(result);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpGet("only-request-types/{requestType}")]
        public async Task<IActionResult> GetAllWithRequestTypeAsync(string requestType,
            [FromQuery] DataSourceLoadOptions loadOptions)
        {
            if (string.IsNullOrEmpty(requestType))
            {
                return await GetAllAsync(loadOptions);
            }

            try
            {
                var condition = $"{nameof(InventoryRequest.RequestType)} = '{requestType}'";

                var collection = await _inventoryRequestService.GetAllAsync<InventoryRequestWithDetail>(
                    IsAdmin == false, $"{nameof(InventoryRequest.RequestNumber)} DESC",
                    condition);

                if (_inventoryRequestService.IsError)
                    return await ErrorResponse(_inventoryRequestService.Exception.Message);

                var result = await Task.Run(() => DataSourceLoader.Load(collection.AsQueryable(), loadOptions));
                return Ok(result);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpGet]
        public override async Task<IActionResult> GetAllAsync([FromQuery] DataSourceLoadOptions loadOptions)
        {
            try
            {
                var collection =
                    await _inventoryRequestService.GetAllAsync<InventoryRequestWithDetail>(
                        IsAdmin == false, $"{nameof(InventoryRequest.RequestNumber)} DESC");

                if (_inventoryRequestService.IsError)
                    return await ErrorResponse(_inventoryRequestService.Exception.Message);

                var result = await Task.Run(() => DataSourceLoader.Load(collection.AsQueryable(), loadOptions));
                return Ok(result);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpGet("{id:int}/supplier-order")]
        public async Task<IActionResult> GetSupplierOrderAsync(int id)
        {
            try
            {
                var condition = $"{nameof(SupplierOrderReference.InventoryRequestId)} = {id}";
                var supplierOrderReference =
                    await _supplierOrderReferenceService.GetFirstOrDefaultAsync<SupplierOrderReference>(
                        IsAdmin == false, null, condition);
                if (_supplierOrderReferenceService.IsError)
                    return await ErrorResponse(_supplierOrderReferenceService.Exception.Message);
                if (supplierOrderReference == null)
                    return await ErrorResponse("None of related supplier order is found");

                var supplierOrder = await _supplierOrderService.GetAsync<SupplierOrder>(id);
                if (_supplierOrderService.IsError) return await ErrorResponse(_supplierOrderService.Exception.Message);

                return Ok(supplierOrder);
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

        [HttpPost("confirm-receipt")]
        [ClaimRequired(RestrictItems.InventoryRequests, RestrictActions.Create)]
        public async Task<IActionResult> ConfirmReceiptAsync([FromBody] IReadOnlyList<int> collection)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Get inventory request
                var condition1 =
                    $"{nameof(InventoryRequest.Id)} IN ({string.Join(",", collection)})";
                var inventoryRequests =
                    await _inventoryRequestService.GetAllAsync<InventoryRequest>(IsAdmin == false, null,
                        condition1);
                if (_inventoryRequestService.IsError)
                    return await ErrorResponse(_inventoryRequestService.Exception.Message);

                var inventoryRequestCollection = inventoryRequests?.ToList() ?? new List<InventoryRequest>();
                if (inventoryRequestCollection.Count == 0)
                    return await ErrorResponse("None of inventory request needed to be completed");

                var confirmedCount = 0;
                foreach (var inventoryRequest in inventoryRequestCollection)
                {
                    var isPreBooked = false;

                    // Get reference to check if inventory request is from production order that came from customer order
                    condition1 =
                        $"{nameof(ProductionInventoryReference.InventoryRequestLineId)} = {inventoryRequest.Id}";
                    var productionInventoryReference =
                        await _productionInventoryReferenceService.GetFirstOrDefaultAsync<ProductionInventoryReference>(
                            IsAdmin == false, null, condition1);
                    if (_productionInventoryReferenceService.IsError)
                        return await ErrorResponse(_productionInventoryReferenceService.Exception.Message);

                    if (productionInventoryReference != null)
                    {
                        condition1 =
                            $"{nameof(ProductionOrderReference.ProductionOrderId)} = {productionInventoryReference.ProductionOrderId}";
                        var productionOrderReference =
                            await _productionOrderReferenceService.GetFirstOrDefaultAsync<ProductionOrderReference>(
                                IsAdmin == false, null, condition1);
                        if (_productionOrderReferenceService.IsError)
                            return await ErrorResponse(_productionOrderReferenceService.Exception.Message);

                        if (productionOrderReference != null)
                        {
                            condition1 =
                                $"{nameof(ProductionRequestReference.ProductionRequestId)} = {productionOrderReference.ProductionRequestId}";
                            isPreBooked =
                                await _productionRequestReferenceService.IsExistsAsync<ProductionRequestReference>(
                                    IsAdmin == false,
                                    condition1);
                            if (_productionRequestReferenceService.IsError)
                                return await ErrorResponse(_productionRequestReferenceService.Exception.Message);
                        }
                    }

                    // Get inventory request line
                    condition1 = $"{nameof(InventoryRequestLine.InventoryRequestId)} = {inventoryRequest.Id}";
                    var condition2 =
                        $"{nameof(InventoryRequestLine.Quantity)} <> {nameof(InventoryRequestLine.ReadyQuantity)}";
                    var isNotReady =
                        await _inventoryRequestLineService.IsExistsAsync<InventoryRequestLine>(IsAdmin == false,
                            condition1,
                            condition2);
                    if (_inventoryRequestLineService.IsError)
                        return await ErrorResponse(_inventoryRequestLineService.Exception.Message);
                    if (isNotReady) continue;

                    var inventoryRequestLines =
                        await _inventoryRequestLineService.GetAllAsync<InventoryRequestLine>(
                            IsAdmin == false, null, condition1);
                    if (_inventoryRequestLineService.IsError)
                        return await ErrorResponse(_inventoryRequestLineService.Exception.Message);
                    var inventoryRequestLineCollection =
                        inventoryRequestLines?.ToList() ?? new List<InventoryRequestLine>();
                    if (inventoryRequestCollection.Count == 0) continue;

                    foreach (var inventoryRequestLine in inventoryRequestLineCollection)
                    {
                        // Get inventory request line detail
                        condition1 =
                            $"{nameof(InventoryRequestLineDetail.InventoryRequestLineId)} = {inventoryRequestLine.Id}";
                        var inventoryRequestLineDetails =
                            await _inventoryRequestLineDetailService.GetAllAsync<InventoryRequestLineDetail>(
                                IsAdmin == false, null, condition1);
                        if (_inventoryRequestLineDetailService.IsError)
                            return await ErrorResponse(_inventoryRequestLineDetailService.Exception.Message);
                        var inventoryRequestLineDetailCollection = inventoryRequestLineDetails?.ToList() ??
                                                                   new List<InventoryRequestLineDetail>();
                        if (inventoryRequestLineDetailCollection.Count == 0)
                            return await ErrorResponse("None of inventory request line detail is available");

                        foreach (var stockTransaction in inventoryRequestLineDetailCollection.Select(
                            inventoryRequestLineDetail => new StockTransaction
                            {
                                TransactionDate = DateTime.Today,
                                TransactionType = TgStockTransactionTypes.In.Value,
                                ProductId = inventoryRequestLineDetail.ProductId,
                                StockroomId = inventoryRequestLineDetail.StockroomId,
                                StockLocationId = inventoryRequestLineDetail.StockLocationId,
                                PackageTypeId = inventoryRequestLineDetail.PackageTypeId,
                                LotNumber = inventoryRequestLineDetail.LotNumber,
                                PackageNumber = inventoryRequestLineDetail.PackageNumber,
                                PalletNo = inventoryRequestLineDetail.PalletNo,
                                Quantity = inventoryRequestLineDetail.Quantity,
                                Cost = 0m,
                                IsAdjustStock = false

                                // TODO: Need to add cost here
                            }))
                        {
                            await _stockTransactionService.InsertUpdateAsync(stockTransaction);
                            if (_stockTransactionService.IsError)
                                return await ErrorResponse(_stockTransactionService.Exception.Message);
                        }
                    }

                    inventoryRequest.Status = TgOrderStatuses.Completed.Value;
                    await _inventoryRequestService.InsertUpdateAsync(inventoryRequest);
                    if (_inventoryRequestService.IsError)
                        return await ErrorResponse(_inventoryRequestService.Exception.Message);

                    confirmedCount++;
                }

                if (confirmedCount == 0) return Ok(false);

                scope.Complete();
                return Ok(true);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpPost("confirm-delivery")]
        [ClaimRequired(RestrictItems.InventoryRequests, RestrictActions.Create)]
        public async Task<IActionResult> ConfirmDeliveryAsync([FromBody] IReadOnlyList<int> collection)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Get inventory request
                var condition1 = $"{nameof(InventoryRequestWithDetail.Id)} IN ({string.Join(",", collection)})";
                var inventoryRequests =
                    await _inventoryRequestService.GetAllAsync<InventoryRequestWithDetail>(
                        IsAdmin == false, null, condition1);
                if (_inventoryRequestService.IsError)
                    return await ErrorResponse(_inventoryRequestService.Exception.Message);

                var inventoryRequestCollection = inventoryRequests?.ToList() ?? new List<InventoryRequestWithDetail>();
                if (inventoryRequestCollection.Count == 0)
                    return await ErrorResponse("None of inventory request needed to be completed");

                var confirmedCount = 0;
                foreach (var inventoryRequest in inventoryRequestCollection)
                {
                    if (inventoryRequest.CanGenerate == false) continue;

                    if (inventoryRequest.Status != TgOrderStatuses.ReadyToPickup.Value &&
                        inventoryRequest.Status != TgOrderStatuses.OnDelivery.Value) continue;

                    var model = new InventoryRequest();
                    model.FromViewModel(inventoryRequest);
                    if (inventoryRequest.Status == TgOrderStatuses.ReadyToPickup.Value &&
                        inventoryRequest.RequestType != TgInventoryRequestTypes.ManualRequest.Value &&
                        inventoryRequest.RequestType != TgInventoryRequestTypes.ManualReturn.Value)
                    {
                        model.Status = TgOrderStatuses.OnDelivery.Value;
                        await _inventoryRequestService.InsertUpdateAsync(model);
                        if (_inventoryRequestService.IsError)
                            return await ErrorResponse(_inventoryRequestService.Exception.Message);
                        confirmedCount++;
                        continue;
                    }

                    // Get target stockroom
                    Stockroom stockroom = null;
                    if (inventoryRequest.RequestType == TgInventoryRequestTypes.CustomerOrder.Value)
                    {
                        stockroom = await _stockroomService.GetByNameAsync("stock out");
                        if (_stockroomService.IsError) return await ErrorResponse(_stockroomService.Exception.Message);
                    }

                    // Get inventory request line
                    condition1 = $"{nameof(InventoryRequestLineWithDetail.InventoryRequestId)} = {inventoryRequest.Id}";
                    var condition2 =
                        $"{nameof(InventoryRequestLineWithDetail.Quantity)} <> {nameof(InventoryRequestLineWithDetail.BookedQuantity)}";
                    var isNotReady = await _inventoryRequestLineService.IsExistsAsync<InventoryRequestLineWithDetail>(
                        IsAdmin == false, condition1, condition2);
                    if (_inventoryRequestLineService.IsError)
                        return await ErrorResponse(_inventoryRequestLineService.Exception.Message);
                    if (isNotReady) continue;

                    var inventoryRequestLines =
                        await _inventoryRequestLineService.GetAllAsync<InventoryRequestLine>(IsAdmin == false, null,
                            condition1);
                    if (_inventoryRequestLineService.IsError)
                        return await ErrorResponse(_inventoryRequestLineService.Exception.Message);
                    var inventoryRequestLineCollection =
                        inventoryRequestLines?.ToList() ?? new List<InventoryRequestLine>();
                    if (inventoryRequestLineCollection.Count == 0) continue;

                    foreach (var inventoryRequestLine in inventoryRequestLineCollection)
                    {
                        // Get inventory request line detail
                        condition1 =
                            $"{nameof(InventoryRequestLineDetail.InventoryRequestLineId)} = {inventoryRequestLine.Id}";
                        var inventoryRequestLineDetails =
                            await _inventoryRequestLineDetailService.GetAllAsync<InventoryRequestLineDetail>(
                                IsAdmin == false, null, condition1);
                        if (_inventoryRequestLineDetailService.IsError)
                            return await ErrorResponse(_inventoryRequestLineDetailService.Exception.Message);
                        var inventoryRequestLineDetailCollection = inventoryRequestLineDetails?.ToList() ??
                                                                   new List<InventoryRequestLineDetail>();
                        if (inventoryRequestLineDetailCollection.Count == 0)
                            return await ErrorResponse("None of inventory request line detail is available");

                        foreach (var inventoryRequestLineDetail in inventoryRequestLineDetailCollection)
                        {
                            var stockTransaction = new StockTransaction
                            {
                                TransactionDate = DateTime.Today,
                                TransactionType = TgStockTransactionTypes.Out.Value,
                                ProductId = inventoryRequestLineDetail.ProductId,
                                StockroomId = inventoryRequestLineDetail.StockroomId,
                                StockLocationId = inventoryRequestLineDetail.StockLocationId,
                                PackageTypeId = inventoryRequestLineDetail.PackageTypeId,
                                LotNumber = inventoryRequestLineDetail.LotNumber,
                                PackageNumber = inventoryRequestLineDetail.PackageNumber,
                                PalletNo = inventoryRequestLineDetail.PalletNo,
                                Quantity = inventoryRequestLineDetail.Quantity,
                                Cost = 0m,
                                IsAdjustStock = false

                                // TODO: Need to add cost here
                            };

                            stockTransaction = await _stockTransactionService.InsertUpdateAsync(stockTransaction);
                            if (_stockTransactionService.IsError)
                                return await ErrorResponse(_stockTransactionService.Exception.Message);

                            if (stockroom != null)
                            {
                                stockTransaction.StockroomId = stockroom.Id;
                                await _stockTransactionService.InsertUpdateAsync(stockTransaction);
                                if (_stockTransactionService.IsError)
                                    return await ErrorResponse(_stockTransactionService.Exception.Message);
                            }

                            inventoryRequestLineDetail.ConfirmQuantity = inventoryRequestLineDetail.Quantity;
                            await _inventoryRequestLineDetailService.InsertUpdateAsync(inventoryRequestLineDetail);
                            if (_inventoryRequestLineDetailService.IsError)
                                return await ErrorResponse(_inventoryRequestLineDetailService.Exception.Message);

                            if (inventoryRequest.RequestType != TgInventoryRequestTypes.CustomerOrder.Value) continue;

                            var bookingReference =
                                await _stockBookingReferenceService.GetAsync<StockBookingReference>(
                                    inventoryRequestLineDetail.InventoryRequestLineId);
                            if (_stockBookingReferenceService.IsError)
                                return await ErrorResponse(_stockBookingReferenceService.Exception.Message);

                            bookingReference.CompletedDate = DateTime.Now;
                            await _stockBookingReferenceService.InsertUpdateAsync(bookingReference);
                            if (_stockBookingReferenceService.IsError)
                                return await ErrorResponse(_stockBookingReferenceService.Exception.Message);
                        }
                    }

                    model.Status = TgOrderStatuses.Completed.Value;
                    await _inventoryRequestService.InsertUpdateAsync(model);
                    if (_inventoryRequestService.IsError)
                        return await ErrorResponse(_inventoryRequestService.Exception.Message);

                    confirmedCount++;
                }

                if (confirmedCount == 0) return Ok(false);

                scope.Complete();
                return Ok(true);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpPost]
        [ClaimRequired(RestrictItems.InventoryRequests, RestrictActions.Create)]
        public override async Task<IActionResult> InsertAsync([FromBody] InventoryRequest model)
        {
            return await base.InsertAsync(model);
        }

        [HttpPatch]
        [ClaimRequired(RestrictItems.InventoryRequests, RestrictActions.Update)]
        public override async Task<IActionResult> UpdateAsync([FromBody] InventoryRequest model)
        {
            return await base.UpdateAsync(model);
        }

        [HttpDelete]
        [ClaimRequired(RestrictItems.InventoryRequests, RestrictActions.Delete)]
        public override async Task<IActionResult> DeleteAsync([FromBody] int id)
        {
            return await base.DeleteAsync(id);
        }

        [HttpDelete("bulk-delete")]
        [ClaimRequired(RestrictItems.InventoryRequests, RestrictActions.Delete)]
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

        private async Task<IActionResult> ConfirmAsync(IEnumerable<int> collection, string transactionType)
        {
            var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var condition = $"{nameof(InventoryRequest.Id)} IN ({string.Join(",", collection)})";
                var requestCollection =
                    await _inventoryRequestService.GetAllAsync<InventoryRequest>(IsAdmin == false, null,
                        condition);
                if (_inventoryRequestService.IsError)
                    return await ErrorResponse(_inventoryRequestService.Exception.Message);

                foreach (var inventoryRequest in requestCollection)
                {
                    var condition1 = $"{nameof(InventoryRequestLine.InventoryRequestId)} = {inventoryRequest.Id}";
                    var condition2 =
                        $"{nameof(InventoryRequestLine.Quantity)} <> {nameof(InventoryRequestLine.ReadyQuantity)}";
                    var isNotCompleted =
                        await _inventoryRequestLineService.IsExistsAsync<InventoryRequestLine>(IsAdmin == false,
                            condition1, condition2);
                    if (_inventoryRequestLineService.IsError)
                        return await ErrorResponse(_inventoryRequestLineService.Exception.Message);

                    if (isNotCompleted) continue;

                    var lineCollection = await
                        _inventoryRequestLineService.GetAllAsync<InventoryRequestLine>(IsAdmin == false, null,
                            condition1);
                    if (_inventoryRequestLineService.IsError)
                        return await ErrorResponse(_inventoryRequestLineService.Exception.Message);

                    foreach (var requestLine in lineCollection)
                    {
                        if ((requestLine?.ProductId ?? 0) == 0) continue;

                        var condition3 =
                            $"{nameof(InventoryRequestLineDetail.InventoryRequestLineId)} = {requestLine.Id}";
                        var detailCollection =
                            await _inventoryRequestLineDetailService.GetAllAsync<InventoryRequestLineDetail>(
                                IsAdmin == false, null, condition3);
                        if (_inventoryRequestLineDetailService.IsError)
                            return await ErrorResponse(_inventoryRequestLineDetailService.Exception.Message);

                        foreach (var lineDetail in detailCollection)
                        {
                            // Create stock transaction
                            var stockTransaction = new StockTransaction
                            {
                                TransactionDate = DateTime.Today,
                                TransactionType = transactionType,
                                ProductId = lineDetail.ProductId,
                                StockroomId = lineDetail.StockroomId,
                                StockLocationId = lineDetail.StockLocationId,
                                PackageTypeId = lineDetail.PackageTypeId,
                                LotNumber = lineDetail.LotNumber,
                                PackageNumber = lineDetail.PackageNumber,
                                PalletNo = lineDetail.PalletNo,
                                Quantity = lineDetail.Quantity,
                                IsAdjustStock = false
                            };

                            await _stockTransactionService.InsertUpdateAsync(stockTransaction);
                            if (_stockTransactionService.IsError)
                                return await ErrorResponse(_stockTransactionService.Exception.Message);
                        }

                        requestLine.Status = TgOrderStatuses.ReadyToPickup.Value;
                        await _inventoryRequestLineService.InsertUpdateAsync(requestLine);
                        if (_inventoryRequestLineService.IsError)
                            return await ErrorResponse(_inventoryRequestLineService.Exception.Message);
                    }

                    inventoryRequest.Status = Equals(transactionType, TgStockTransactionTypes.In.Value)
                        ? TgOrderStatuses.Completed.Value
                        : TgOrderStatuses.ReadyToPickup.Value;

                    await _inventoryRequestService.InsertUpdateAsync(inventoryRequest);
                    if (_inventoryRequestService.IsError)
                        return await ErrorResponse(_inventoryRequestService.Exception.Message);
                }

                scope.Complete();
                return Ok(true);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }
    }
}