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
    [Route("api/production-orders")]
    public class ProductionOrdersController : TgControllerBase<ProductionOrderWithDetail, ProductionOrder>
    {
        private readonly IProductionOrderService _productionOrderService;
        private readonly IProductionRequestService _productionRequestService;
        private readonly IProductionOrderReferenceService _productionOrderReferenceService;
        private readonly IInventoryRequestService _inventoryRequestService;
        private readonly IInventoryRequestLineService _inventoryRequestLineService;
        private readonly IProductService _productService;
        private readonly IProductionInventoryReferenceService _productionInventoryReferenceService;
        private readonly IStockroomService _stockroomService;
        private readonly IProductionOrderScheduleService _productionOrderScheduleService;

        public ProductionOrdersController(IServiceProvider serviceProvider,
            IProductionOrderService productionOrderService, IProductionRequestService productionRequestService,
            IProductionOrderReferenceService productionOrderReferenceService,
            IInventoryRequestService inventoryRequestService, IInventoryRequestLineService inventoryRequestLineService,
            IProductService productService, IProductionInventoryReferenceService productionInventoryReferenceService,
            IStockroomService stockroomService, IProductionOrderScheduleService productionOrderScheduleService,
            ILogger<ProductionOrdersController> logger)
            : base(serviceProvider, productionOrderService, logger)
        {
            _productionOrderService = productionOrderService;
            _productionRequestService = productionRequestService;
            _productionOrderReferenceService = productionOrderReferenceService;
            _inventoryRequestService = inventoryRequestService;
            _inventoryRequestLineService = inventoryRequestLineService;
            _productService = productService;
            _productionInventoryReferenceService = productionInventoryReferenceService;
            _stockroomService = stockroomService;
            _productionOrderScheduleService = productionOrderScheduleService;
        }

        [HttpGet("search")]
        public override Task<IActionResult> SearchAsync([FromQuery] string[] columns,
            [FromQuery] string[] searchStrings, [FromQuery] string[] orderColumns = null)
        {
            return base.SearchAsync(columns, searchStrings, orderColumns);
        }

        [HttpGet("schedules")]
        public async Task<IActionResult> GetTransportationRequestScheduleAsync()
        {
            try
            {
                var collection =
                    await _productionOrderScheduleService.GetAllAsync<ProductionOrderSchedule>(false, "StartDate");

                if (_productionOrderScheduleService.IsError)
                    return await ErrorResponse(_productionOrderScheduleService.Exception.Message);

                return Ok(collection);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpGet]
        public override async Task<IActionResult> GetAllAsync(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var collection = await _productionOrderService.GetAllAsync<ProductionOrderWithDetail>(
                    IsAdmin == false, $"{nameof(ProductionOrder.OrderNumber)} DESC");
                if (_productionOrderService.IsError)
                    return await ErrorResponse(_productionOrderService.Exception.Message);

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

        [HttpPost("can-generate-requests")]
        public async Task<IActionResult> CanGenerateRequests([FromBody] IReadOnlyList<int> collection)
        {
            try
            {
                var condition1 = $"{nameof(ProductionOrderWithDetail.Id)} IN ({string.Join(",", collection)})";
                var condition2 =
                    $"{nameof(ProductionOrderWithDetail.DeliveredQuantity)} < {nameof(ProductionOrderWithDetail.ReadyQuantity)}";
                var result = await _productionOrderService.IsExistsAsync<ProductionOrderWithDetail>(IsAdmin == false,
                    condition1, condition2);
                if (_productionOrderService.IsError)
                    return await ErrorResponse(_productionOrderService.Exception.Message);

                return Ok(result);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpPost("generate-request")]
        [ClaimRequired(RestrictItems.ProductionOrders, RestrictActions.Create)]
        public async Task<IActionResult> GenerateInventoryRequest([FromBody] IReadOnlyList<int> collection)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var listOfId = string.Join(",", collection.Select(id => id.ToString()));
                var condition = $"Id IN ({listOfId})";
                var result =
                    await _productionOrderService.GetAllAsync<ProductionOrderWithDetail>(IsAdmin == false, null,
                        condition);
                if (_productionOrderService.IsError)
                    return await ErrorResponse(_productionOrderService.Exception.Message);

                var productionOrderCollection = result?.ToList() ?? new List<ProductionOrderWithDetail>();
                if (productionOrderCollection.Count == 0)
                    return await ErrorResponse("None of production order is selected");

                var canGenerateCount = productionOrderCollection.Count(o => o.DeliveredQuantity < o.ReadyQuantity);
                if (canGenerateCount == 0) return Ok(false);

                var stockroom = await _stockroomService.GetByNameAsync("finished goods");
                if (_stockroomService.IsError) return await ErrorResponse("Cannot find stockroom");

                var dueDate = productionOrderCollection.Min(req => req.DueDate);
                var inventoryRequest = new InventoryRequest
                {
                    RequestDate = DateTime.Today,
                    DueDate = dueDate < DateTime.Today ? DateTime.Today : dueDate,
                    Status = TgOrderStatuses.New.Value,
                    StatusDate = DateTime.Today,
                    RequestType = TgInventoryRequestTypes.ProductionOrder.Value,
                    StockroomId = stockroom.Id,
                    RequestedBy = _productionOrderService.CurrentUsername,
                    Remark =
                        $"Production order {string.Join(", ", productionOrderCollection.Select(o => $"#{o.OrderNumber:000000}"))}"
                };

                inventoryRequest = await _inventoryRequestService.InsertUpdateAsync(inventoryRequest);
                if (_inventoryRequestService.IsError)
                    return await ErrorResponse(_inventoryRequestService.Exception.Message);

                foreach (var productionOrder in productionOrderCollection)
                {
                    var product = await _productService.GetAsync<Product>(productionOrder.ProductId, IsAdmin == false);
                    if (_productService.IsError) return await ErrorResponse(_productService.Exception.Message);
                    if (product == null)
                        return await ErrorResponse($"Product Id {productionOrder.ProductId} not found");

                    var newQuantity = productionOrder.ReadyQuantity - productionOrder.DeliveredQuantity;
                    var inventoryRequestLine = new InventoryRequestLine
                    {
                        InventoryRequestId = inventoryRequest.Id,
                        ProductId = productionOrder.ProductId,
                        Description = product.Name,
                        Quantity = newQuantity,
                        Status = TgOrderStatuses.New.Value,
                        StatusDate = DateTime.Today
                    };

                    inventoryRequestLine = await _inventoryRequestLineService.InsertUpdateAsync(inventoryRequestLine);
                    if (_inventoryRequestLineService.IsError)
                        return await ErrorResponse(_inventoryRequestLineService.Exception.Message);

                    // set reference
                    var productionInventoryReference = new ProductionInventoryReference
                    {
                        ProductionOrderId = productionOrder.Id,
                        InventoryRequestLineId = inventoryRequestLine.Id,
                        Quantity = newQuantity
                    };

                    await _productionInventoryReferenceService.InsertUpdateAsync(productionInventoryReference);
                    if (_productionInventoryReferenceService.IsError)
                        return await ErrorResponse(_productionInventoryReferenceService.Exception.Message);

                    if (productionOrder.Status != TgOrderStatuses.ReadyToPickup.Value) continue;

                    // Update completed status
                    var order = await _productionOrderService.GetAsync<ProductionOrder>(productionOrder.Id);
                    if (_productionOrderService.IsError)
                        return await ErrorResponse(_productionOrderService.Exception.Message);

                    order.Status = TgOrderStatuses.Completed.Value;
                    await _productionOrderService.InsertUpdateAsync(order);
                    if (_productionOrderService.IsError)
                        return await ErrorResponse(_productionOrderService.Exception.Message);

                    // Get request reference
                    condition = $"{nameof(ProductionOrderReference.ProductionOrderId)} = {productionOrder.Id}";
                    var productionOrderReferences =
                        await _productionOrderReferenceService.GetAllAsync<ProductionOrderReference>(IsAdmin == false,
                            null, condition);
                    if (_productionOrderReferenceService.IsError)
                        return await ErrorResponse(_productionOrderReferenceService.Exception.Message);

                    var productionOrderReferenceCollection =
                        productionOrderReferences?.ToList() ?? new List<ProductionOrderReference>();
                    if (productionOrderReferenceCollection.Count == 0) continue;

                    // Update status related
                    foreach (var productionOrderReference in productionOrderReferenceCollection)
                    {
                        var productionRequest =
                            await _productionRequestService.GetAsync<ProductionRequest>(productionOrderReference
                                .ProductionRequestId);
                        if (_productionRequestService.IsError)
                            return await ErrorResponse(_productionRequestService.Exception.Message);

                        productionRequest.Status = TgOrderStatuses.Completed.Value;
                        await _productionRequestService.InsertUpdateAsync(productionRequest);
                        if (_productionRequestService.IsError)
                            return await ErrorResponse(_productionRequestService.Exception.Message);
                    }
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
        [ClaimRequired(RestrictItems.ProductionOrders, RestrictActions.Create)]
        public override async Task<IActionResult> InsertAsync([FromBody] ProductionOrder model)
        {
            return await base.InsertAsync(model);
        }

        [HttpPatch]
        [ClaimRequired(RestrictItems.ProductionOrders, RestrictActions.Update)]
        public override async Task<IActionResult> UpdateAsync([FromBody] ProductionOrder model)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var result = await base.UpdateAsync(model);
                if (IsError) return result;

                if (model.Status != TgOrderStatuses.Completed.Value)
                {
                    scope.Complete();
                    return result;
                }

                var productionOrderReferences =
                    await _productionOrderReferenceService.GetAllAsync<ProductionOrderReference>(IsAdmin == false, null,
                        $"{nameof(ProductionOrderReference.ProductionOrderId)} = {model.Id}");
                if (_productionOrderReferenceService.IsError)
                    return await ErrorResponse(_productionOrderReferenceService.Exception.Message);

                var idCollection = string.Join(",",
                    productionOrderReferences.Select(r => r.ProductionRequestId.ToString()));
                var condition = $"Id IN ({idCollection})";
                var productionRequests =
                    await _productionRequestService.GetAllAsync<ProductionRequest>(IsAdmin == false, null,
                        condition);
                if (_productionRequestService.IsError)
                    return await ErrorResponse(_productionRequestService.Exception.Message);

                foreach (var productionRequest in productionRequests)
                {
                    productionRequest.Status = TgOrderStatuses.Completed.Value;
                    await _productionRequestService.InsertUpdateAsync(productionRequest);
                    if (_productionRequestService.IsError == false) continue;

                    return await ErrorResponse(_productionRequestService.Exception.Message);
                }

                scope.Complete();
                return result;
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpDelete]
        [ClaimRequired(RestrictItems.ProductionOrders, RestrictActions.Delete)]
        public override async Task<IActionResult> DeleteAsync([FromBody] int id)
        {
            return await base.DeleteAsync(id);
        }

        [HttpDelete("bulk-delete")]
        [ClaimRequired(RestrictItems.ProductionOrders, RestrictActions.Delete)]
        public override async Task<IActionResult> BulkDeleteAsync([FromBody] IReadOnlyList<int> collection)
        {
            return await base.BulkDeleteAsync(collection);
        }

        [HttpPut]
        [AuthorizeRequired(RestrictRoles.Administrator, RestrictRoles.Director)]
        public override Task<IActionResult> RestoreAsync([FromBody] int id)
        {
            return base.RestoreAsync(id);
        }

        [HttpPut("bulk-restore")]
        [AuthorizeRequired(RestrictRoles.Administrator, RestrictRoles.Director)]
        public override Task<IActionResult> BulkRestoreAsync([FromBody] IReadOnlyList<int> collection)
        {
            return base.BulkRestoreAsync(collection);
        }

        [HttpPost("unique-validation")]
        public override async Task<IActionResult> IsUniqueAsync([FromBody] ValidationRequest model)
        {
            return await base.IsUniqueAsync(model);
        }
    }
}