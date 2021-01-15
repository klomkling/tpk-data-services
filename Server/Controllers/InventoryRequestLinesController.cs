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
    [Route("api/inventory-request-lines")]
    public class
        InventoryRequestLinesController : TgControllerBase<InventoryRequestLineWithDetail, InventoryRequestLine>
    {
        private readonly IInventoryRequestService _inventoryRequestService;
        private readonly IInventoryRequestLineService _inventoryRequestLineService;
        private readonly IInventoryRequestLineDetailService _inventoryRequestLineDetailService;
        private readonly ICustomerOrderLineReferenceService _customerOrderLineReferenceService;
        private readonly ICustomerOrderLineService _customerOrderLineService;
        private readonly ICustomerProductService _customerProductService;
        private readonly ICustomerProductPackageService _customerProductPackageService;
        private readonly IStockService _stockService;
        private readonly IStockBookingDetailReferenceService _stockBookingDetailReferenceService;
        private readonly IProductionInventoryReferenceService _productionInventoryReferenceService;
        private readonly IProductionOrderService _productionOrderService;

        public InventoryRequestLinesController(IServiceProvider serviceProvider,
            IInventoryRequestService inventoryRequestService,
            IInventoryRequestLineService inventoryRequestLineService,
            IInventoryRequestLineDetailService inventoryRequestLineDetailService,
            ICustomerOrderLineReferenceService customerOrderLineReferenceService,
            ICustomerOrderLineService customerOrderLineService, ICustomerProductService customerProductService,
            ICustomerProductPackageService customerProductPackageService,
            IStockService stockService, IStockBookingDetailReferenceService stockBookingDetailReferenceService,
            IProductionInventoryReferenceService productionInventoryReferenceService,
            IProductionOrderService productionOrderService,
            ILogger<InventoryRequestLinesController> logger)
            : base(serviceProvider, inventoryRequestLineService, logger)
        {
            _inventoryRequestService = inventoryRequestService;
            _inventoryRequestLineService = inventoryRequestLineService;
            _inventoryRequestLineDetailService = inventoryRequestLineDetailService;
            _customerOrderLineReferenceService = customerOrderLineReferenceService;
            _customerOrderLineService = customerOrderLineService;
            _customerProductService = customerProductService;
            _customerProductPackageService = customerProductPackageService;
            _stockService = stockService;
            _stockBookingDetailReferenceService = stockBookingDetailReferenceService;
            _productionInventoryReferenceService = productionInventoryReferenceService;
            _productionOrderService = productionOrderService;
        }

        [HttpGet("search")]
        public override Task<IActionResult> SearchAsync([FromQuery] string[] columns,
            [FromQuery] string[] searchStrings, [FromQuery] string[] orderColumns = null)
        {
            return base.SearchAsync(columns, searchStrings, orderColumns);
        }

        [HttpGet("{id:int}/details")]
        public async Task<IActionResult> GetDetailsAsync(int id, [FromQuery] DataSourceLoadOptions loadOptions)
        {
            try
            {
                var condition = $"{nameof(InventoryRequestLineDetailWithDetail.InventoryRequestLineId)} = {id}";

                var collection =
                    await _inventoryRequestLineDetailService.GetAllAsync<InventoryRequestLineDetailWithDetail>(
                        IsAdmin == false, null, condition);

                if (_inventoryRequestLineDetailService.IsError)
                    return await ErrorResponse(_inventoryRequestLineDetailService.Exception.Message);

                var result = await Task.Run(() => DataSourceLoader.Load(collection.AsQueryable(), loadOptions));
                return Ok(result);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpGet("{id:int}/customer-product")]
        public async Task<IActionResult> GetCustomerProductPackages(int id)
        {
            try
            {
                // Get inventory request line
                var inventoryRequestLine = await _inventoryRequestLineService.GetAsync<InventoryRequestLine>(id);
                if (_inventoryRequestLineService.IsError)
                    return await ErrorResponse(_inventoryRequestLineService.Exception.Message);
                if (inventoryRequestLine == null)
                    return await ErrorResponse("None of selected inventory request line is available");

                // Get inventory request
                var inventoryRequest =
                    await _inventoryRequestService.GetAsync<InventoryRequest>(inventoryRequestLine.InventoryRequestId);
                if (_inventoryRequestService.IsError)
                    return await ErrorResponse(_inventoryRequestService.Exception.Message);
                if (inventoryRequest == null)
                    return await ErrorResponse("None of selected inventory request is available");

                // Check if not customer order
                // TODO: Change of Package Type
                if (inventoryRequest.RequestType != TgInventoryRequestTypes.CustomerOrder.Value)
                    return Ok(new List<Product>());

                // Get customer order line reference
                var condition =
                    $"{nameof(CustomerOrderLineReference.InventoryRequestLineId)} = {inventoryRequestLine.Id}";
                var customerOrderLineReference =
                    await _customerOrderLineReferenceService.GetFirstOrDefaultAsync<CustomerOrderLineReference>(false,
                        null,
                        condition);
                if (_customerOrderLineReferenceService.IsError)
                    return await ErrorResponse(_customerOrderLineReferenceService.Exception.Message);

                if (customerOrderLineReference == null)
                    return await ErrorResponse("None of customer order line reference is available");

                // Get customer order line
                var customerOrderLine =
                    await _customerOrderLineService.GetAsync<CustomerOrderLine>(customerOrderLineReference
                        .CustomerOrderLineId);
                if (_customerOrderLineService.IsError)
                    return await ErrorResponse(_customerOrderLineService.Exception.Message);
                if (customerOrderLine == null) return await ErrorResponse("None of customer order line is available");

                // Get customer product
                var customerProduct =
                    await _customerProductService.GetAsync<CustomerProduct>(customerOrderLine.CustomerProductId);
                if (_customerProductService.IsError)
                    return await ErrorResponse(_customerProductService.Exception.Message);
                if (customerProduct == null) return await ErrorResponse("None of customer product is available");

                return Ok(customerProduct);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpGet("{id:int}/production-order")]
        public async Task<IActionResult> GetProductionOrder(int id)
        {
            try
            {
                var condition = $"{nameof(ProductionInventoryReference.InventoryRequestLineId)} = {id}";
                var productionInventoryReference =
                    await _productionInventoryReferenceService.GetFirstOrDefaultAsync<ProductionInventoryReference>(
                        IsAdmin == false, null, condition);
                if (_productionInventoryReferenceService.IsError)
                    return await ErrorResponse(_productionInventoryReferenceService.Exception.Message);

                if (productionInventoryReference == null) return NoContent();

                var productionOrder =
                    await _productionOrderService.GetAsync<ProductionOrder>(productionInventoryReference
                        .ProductionOrderId);
                if (_productionOrderService.IsError)
                    return await ErrorResponse(_productionOrderService.Exception.Message);

                return productionOrder == null ? NoContent() : (IActionResult) Ok(productionOrder);
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

        [HttpPost("{id:int}/bookings")]
        [ClaimRequired(RestrictItems.InventoryRequestLines, RestrictActions.Create)]
        public async Task<IActionResult> BookingAsync(int id, [FromBody] IReadOnlyList<int> collection)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Get inventory request line
                var inventoryRequestLine =
                    await _inventoryRequestLineService.GetAsync<InventoryRequestLine>(id, IsAdmin == false);
                if (_inventoryRequestLineService.IsError)
                    return await ErrorResponse(_inventoryRequestLineService.Exception.Message);

                // If no remain quantity do nothing
                if (inventoryRequestLine.RemainQuantity == 0) return Ok(true);

                // Get inventory request
                var inventoryRequest =
                    await _inventoryRequestService.GetAsync<InventoryRequest>(inventoryRequestLine.InventoryRequestId,
                        IsAdmin == false);
                if (_inventoryRequestService.IsError)
                    return await ErrorResponse(_inventoryRequestService.Exception.Message);
                if (inventoryRequest == null) return await ErrorResponse("Referencing inventory request was not found");

                var condition3 = string.Empty;
                if (inventoryRequest.RequestType == TgInventoryRequestTypes.CustomerOrder.Value)
                {
                    // Get reference
                    condition3 = $"{nameof(CustomerOrderLineReference.InventoryRequestLineId)} = {id}";
                    var customerOrderLineReference =
                        await _customerOrderLineReferenceService.GetFirstOrDefaultAsync<CustomerOrderLineReference>(
                            false, null, condition3);
                    if (_customerOrderLineReferenceService.IsError)
                        return await ErrorResponse(_customerOrderLineReferenceService.Exception.Message);
                    if (customerOrderLineReference == null)
                        return await ErrorResponse("None of customer order line reference is available");

                    // Get customer order line
                    var customerOrderLine =
                        await _customerOrderLineService.GetAsync<CustomerOrderLine>(
                            customerOrderLineReference.CustomerOrderLineId, IsAdmin == false);
                    if (_customerOrderLineService.IsError)
                        return await ErrorResponse(_customerOrderLineService.Exception.Message);
                    if (customerOrderLine == null)
                        return await ErrorResponse("Referencing customer order line was not found");

                    // Get customer product
                    condition3 =
                        $"{nameof(CustomerProductPackage.CustomerProductId)} = {customerOrderLine.CustomerProductId}";
                    var customerProductPackages =
                        await _customerProductPackageService.GetAllAsync<CustomerProductPackage>(IsAdmin == false, null,
                            condition3);
                    if (_customerProductPackageService.IsError)
                        return await ErrorResponse(_customerProductPackageService.Exception.Message);
                    var customerProductPackageCollection =
                        customerProductPackages?.ToList() ?? new List<CustomerProductPackage>();

                    condition3 = customerProductPackageCollection.Count > 0
                        ? $"{nameof(Stock.PackageTypeId)} IN ({string.Join(",", customerProductPackageCollection.Select(c => c.PackageTypeId.ToString()))})"
                        : null;
                }

                // Get all stock available
                var listOfId = string.Join(",", collection.Select(i => i.ToString()));
                var condition = $"{nameof(StockWithDetail.Id)} IN ({listOfId})";
                var condition2 = $"{nameof(StockWithDetail.AvailableQuantity)} > 0";
                var orderClause = $"{nameof(StockWithDetail.ReceivedDate)}, {nameof(StockWithDetail.PalletNo)}, " +
                                  $"{nameof(StockWithDetail.PackageNumber)}, {nameof(StockWithDetail.LotNumber)}";
                var stocks = await _stockService.GetAllAsync<StockWithDetail>(IsAdmin == false, orderClause,
                    condition, condition2, condition3);
                if (_stockService.IsError) return await ErrorResponse(_stockService.Exception.Message);

                var stockCollection = stocks?.ToList() ?? new List<StockWithDetail>();
                if (stockCollection.Count == 0)
                    return await ErrorResponse("Stock is not available");

                var qty = inventoryRequestLine.RemainQuantity;
                foreach (var stock in stockCollection)
                {
                    var inventoryRequestLineDetail = new InventoryRequestLineDetail
                    {
                        InventoryRequestLineId = inventoryRequestLine.Id,
                        ProductId = stock.ProductId,
                        StockroomId = stock.StockroomId,
                        StockLocationId = stock.StockLocationId,
                        PackageTypeId = stock.PackageTypeId,
                        PackageNumber = stock.PackageNumber,
                        PalletNo = stock.PalletNo,
                        LotNumber = stock.LotNumber
                    };

                    var stockBookingDetailReference = new StockBookingDetailReference
                    {
                        StockId = stock.Id,
                        DueDate = inventoryRequest.DueDate
                    };

                    if (stock.AvailableQuantity >= qty)
                    {
                        inventoryRequestLineDetail.Quantity = qty;
                        stockBookingDetailReference.Quantity = qty;
                        qty = 0m;
                    }
                    else
                    {
                        inventoryRequestLineDetail.Quantity = stock.AvailableQuantity;
                        stockBookingDetailReference.Quantity = stock.AvailableQuantity;
                        qty -= stock.AvailableQuantity;
                    }

                    // set inventory request line detail
                    inventoryRequestLineDetail =
                        await _inventoryRequestLineDetailService.InsertUpdateAsync(inventoryRequestLineDetail);
                    if (_inventoryRequestLineDetailService.IsError)
                        return await ErrorResponse(_inventoryRequestLineDetailService.Exception.Message);

                    // set booking detail reference
                    stockBookingDetailReference.InventoryRequestLineDetailId = inventoryRequestLineDetail.Id;
                    await _stockBookingDetailReferenceService.InsertUpdateAsync(stockBookingDetailReference);
                    if (_stockBookingDetailReferenceService.IsError)
                        return await ErrorResponse(_stockBookingDetailReferenceService.Exception.Message);

                    if (qty == 0m) break;
                }

                // Update inventory request status
                condition = $"{nameof(InventoryRequestLine.InventoryRequestId)} = {inventoryRequest.Id}";
                condition2 =
                    $"{nameof(InventoryRequestLine.Quantity)} - {nameof(inventoryRequestLine.ReadyQuantity)} > 0";
                var isNotReady =
                    await _inventoryRequestLineService.IsExistsAsync<InventoryRequestLine>(IsAdmin == false, condition,
                        condition2);
                if (_inventoryRequestLineService.IsError)
                    return await ErrorResponse(_inventoryRequestLineService.Exception.Message);

                inventoryRequest.Status =
                    isNotReady ? TgOrderStatuses.Start.Value : TgOrderStatuses.ReadyToPickup.Value;
                await _inventoryRequestService.InsertUpdateAsync(inventoryRequest);
                if (_inventoryRequestService.IsError)
                    return await ErrorResponse(_inventoryRequestService.Exception.Message);

                scope.Complete();
                return Ok(true);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpPost]
        [ClaimRequired(RestrictItems.InventoryRequestLines, RestrictActions.Create)]
        public override async Task<IActionResult> InsertAsync([FromBody] InventoryRequestLine model)
        {
            return await base.InsertAsync(model);
        }

        [HttpPatch]
        [ClaimRequired(RestrictItems.InventoryRequestLines, RestrictActions.Update)]
        public override async Task<IActionResult> UpdateAsync([FromBody] InventoryRequestLine model)
        {
            return await base.UpdateAsync(model);
        }

        [HttpDelete]
        [ClaimRequired(RestrictItems.InventoryRequestLines, RestrictActions.Delete)]
        public override async Task<IActionResult> DeleteAsync([FromBody] int id)
        {
            return await base.DeleteAsync(id);
        }

        [HttpDelete("bulk-delete")]
        [ClaimRequired(RestrictItems.InventoryRequestLines, RestrictActions.Delete)]
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