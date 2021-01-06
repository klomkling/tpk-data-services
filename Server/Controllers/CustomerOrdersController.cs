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
    [Route("api/customer-orders")]
    public class CustomerOrdersController : TgControllerBase<CustomerOrderWithDetail, CustomerOrder>
    {
        private readonly ICustomerOrderLineService _customerOrderLineService;
        private readonly ICustomerAddressService _customerAddressService;
        private readonly ICustomerContactService _customerContactService;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly IInventoryRequestLineService _inventoryRequestLineService;
        private readonly IInventoryRequestService _inventoryRequestService;
        private readonly IStockBookingReferenceService _stockBookingReferenceService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerOrderReferenceService _customerOrderReferenceService;
        private readonly ICustomerOrderLineReferenceService _customerOrderLineReferenceService;
        private readonly IStockroomService _stockroomService;
        private readonly IProductService _productService;
        private readonly IProductionRequestService _productionRequestService;
        private readonly IProductionRequestReferenceService _productionRequestReferenceService;
        private readonly ITransportationRequestService _transportationRequestService;
        private readonly ITransportationRequestReferenceService _transportationRequestReferenceService;
        private readonly ITransportationRequestLineService _transportationRequestLineService;
        private readonly ITransportationRequestLineReferenceService _transportationRequestLineReferenceService;

        public CustomerOrdersController(IServiceProvider serviceProvider, ICustomerOrderService customerOrderService,
            ICustomerOrderLineService customerOrderLineService, ICustomerAddressService customerAddressService,
            ICustomerContactService customerContactService, IInventoryRequestService inventoryRequestService,
            IInventoryRequestLineService inventoryRequestLineService,
            IStockBookingReferenceService stockBookingReferenceService, ICustomerService customerService,
            ICustomerOrderReferenceService customerOrderReferenceService,
            ICustomerOrderLineReferenceService customerOrderLineReferenceService,
            IStockroomService stockroomService, IProductService productService,
            IProductionRequestService productionRequestService,
            IProductionRequestReferenceService productionRequestReferenceService,
            ITransportationRequestService transportationRequestService,
            ITransportationRequestReferenceService transportationRequestReferenceService,
            ITransportationRequestLineService transportationRequestLineService,
            ITransportationRequestLineReferenceService transportationRequestLineReferenceService,
            ILogger<CustomerOrdersController> logger)
            : base(serviceProvider, customerOrderService, logger)
        {
            _customerOrderService = customerOrderService;
            _customerOrderLineService = customerOrderLineService;
            _customerAddressService = customerAddressService;
            _customerContactService = customerContactService;
            _inventoryRequestService = inventoryRequestService;
            _inventoryRequestLineService = inventoryRequestLineService;
            _stockBookingReferenceService = stockBookingReferenceService;
            _customerService = customerService;
            _customerOrderReferenceService = customerOrderReferenceService;
            _customerOrderLineReferenceService = customerOrderLineReferenceService;
            _stockroomService = stockroomService;
            _productService = productService;
            _productionRequestService = productionRequestService;
            _productionRequestReferenceService = productionRequestReferenceService;
            _transportationRequestService = transportationRequestService;
            _transportationRequestReferenceService = transportationRequestReferenceService;
            _transportationRequestLineService = transportationRequestLineService;
            _transportationRequestLineReferenceService = transportationRequestLineReferenceService;
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
                var condition = $"{nameof(CustomerOrderLine.CustomerOrderId)} = {id}";

                var collection =
                    await _customerOrderLineService.GetAllAsync<CustomerOrderLineWithDetail>(IsAdmin == false, null,
                        condition);

                if (_customerOrderLineService.IsError)
                    return await ErrorResponse(_customerOrderLineService.Exception.Message);

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
                    await _customerOrderService.GetAsync<CustomerOrderWithDetail>(id, IsAdmin == false);

                if (_customerOrderService.IsError == false)
                    return result == null ? NotFound() : (IActionResult) Ok(result);

                return await ErrorResponse(_customerOrderService.Exception.Message);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpGet("inventory-requests/{id:int}")]
        public async Task<IActionResult> GetByInventoryRequestIdAsync(int id)
        {
            try
            {
                var condition = $"InventoryRequestId = {id}";
                var customerOrderReference = await
                    _customerOrderReferenceService.GetFirstOrDefaultAsync<CustomerOrderReference>(false, null,
                        condition);
                if (_customerOrderReferenceService.IsError)
                    return await ErrorResponse(_customerOrderReferenceService.Exception.Message);

                var result =
                    await _customerOrderService.GetAsync<CustomerOrderWithDetail>(
                        customerOrderReference?.CustomerOrderId ?? 0, IsAdmin == false);
                if (_customerOrderService.IsError == false)
                    return result == null ? NotFound() : (IActionResult) Ok(result);

                return await ErrorResponse(_customerOrderService.Exception.Message);
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
                var condition1 = $"{nameof(CustomerOrderWithDetail.Id)} IN ({string.Join(",", collection)})";
                var condition2 = $"{nameof(CustomerOrderWithDetail.CanGenerate)} = 1";
                var result = await _customerOrderService.IsExistsAsync<CustomerOrderWithDetail>(IsAdmin == false,
                    condition1, condition2);
                if (_customerOrderService.IsError) return await ErrorResponse(_customerOrderService.Exception.Message);

                return Ok(result);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpPost("generate-request")]
        [ClaimRequired(RestrictItems.CustomerOrders, RestrictActions.Create)]
        public async Task<IActionResult> GenerateInventoryRequest([FromBody] IReadOnlyList<int> collection)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var listOfId = string.Join(",", collection.Select(id => id.ToString()));
                var condition = $"{nameof(CustomerOrderWithDetail.Id)} IN ({listOfId})";
                var result =
                    await _customerOrderService.GetAllAsync<CustomerOrderWithDetail>(IsAdmin == false, null, condition);
                if (_customerOrderService.IsError) return await ErrorResponse(_customerOrderService.Exception.Message);

                var customerOrders = result?.ToList() ?? new List<CustomerOrderWithDetail>();
                if (customerOrders.Count == 0) return await ErrorResponse("None of customer order is selected");

                var stockroom = await _stockroomService.GetByNameAsync("stock out");
                if (_stockroomService.IsError) return await ErrorResponse("Cannot find stockroom");

                var generatedCount = 0;
                foreach (var customerOrder in customerOrders)
                {
                    if (customerOrder.CanGenerate == false) continue;

                    condition = $"{nameof(CustomerOrderLineWithDetail.CustomerOrderId)} = {customerOrder.Id}";
                    var lineResult =
                        await _customerOrderLineService.GetAllAsync<CustomerOrderLineWithDetail>(IsAdmin == false, null,
                            condition);
                    if (_customerOrderLineService.IsError)
                        return await ErrorResponse(_customerOrderLineService.Exception.Message);

                    var customerOrderLines = lineResult?.ToList() ?? new List<CustomerOrderLineWithDetail>();
                    if (customerOrderLines.Count == 0)
                        return await ErrorResponse("None of customer order lines is available");

                    var customerOrderReference =
                        await _customerOrderReferenceService.GetAsync<CustomerOrderReference>(customerOrder.Id, false);
                    if (_customerOrderReferenceService.IsError)
                        return await ErrorResponse(_customerOrderReferenceService.Exception.Message);

                    InventoryRequest inventoryRequest;
                    TransportationRequest transportationRequest;
                    if (customerOrderReference == null)
                    {
                        inventoryRequest = null;
                        transportationRequest = null;
                    }
                    else
                    {
                        inventoryRequest =
                            await _inventoryRequestService.GetAsync<InventoryRequest>(
                                customerOrderReference.InventoryRequestId,
                                IsAdmin == false);
                        if (_inventoryRequestService.IsError)
                            return await ErrorResponse(_inventoryRequestService.Exception.Message);

                        condition =
                            $"{nameof(TransportationRequestReference.InventoryRequestId)} = {inventoryRequest.Id}";
                        var transportationRequestReference =
                            await _transportationRequestReferenceService
                                .GetFirstOrDefaultAsync<TransportationRequestReference>(IsAdmin == false, condition);
                        if (_transportationRequestReferenceService.IsError)
                            return await ErrorResponse(_transportationRequestReferenceService.Exception.Message);

                        if (transportationRequestReference != null)
                        {
                            transportationRequest =
                                await _transportationRequestService.GetAsync<TransportationRequest>(
                                    transportationRequestReference.InventoryRequestId);
                            if (_transportationRequestService.IsError)
                                return await ErrorResponse(_transportationRequestService.Exception.Message);
                        }
                        else
                        {
                            transportationRequest = null;
                        }
                    }

                    var customer = await _customerService.GetAsync<Customer>(customerOrder.CustomerId);
                    if (_customerService.IsError) return await ErrorResponse(_customerService.Exception.Message);

                    if (inventoryRequest == null)
                    {
                        inventoryRequest = new InventoryRequest
                        {
                            RequestDate = customerOrder.OrderDate,
                            DueDate = customerOrder.DueDate < DateTime.Today ? DateTime.Today : customerOrder.DueDate,
                            Status = TgOrderStatuses.New.Value,
                            StatusDate = DateTime.Today,
                            RequestType = TgInventoryRequestTypes.CustomerOrder.Value,
                            StockroomId = stockroom.Id,
                            RequestedBy = _customerOrderService.CurrentUsername,
                            Remark = $"Customer Order #{customerOrder.OrderNumber:000000} : " +
                                     $"{customer.Name}{(string.IsNullOrEmpty(customerOrder.Comment) ? null : $", {customerOrder.Comment}")}"
                        };

                        inventoryRequest = await _inventoryRequestService.InsertUpdateAsync(inventoryRequest);
                        if (_inventoryRequestService.IsError)
                            return await ErrorResponse(_inventoryRequestService.Exception.Message);

                        customerOrderReference = new CustomerOrderReference
                        {
                            CustomerOrderId = customerOrder.Id,
                            InventoryRequestId = inventoryRequest.Id
                        };

                        await _customerOrderReferenceService.InsertUpdateAsync(customerOrderReference);
                        if (_customerOrderReferenceService.IsError)
                            return await ErrorResponse(_customerOrderReferenceService.Exception.Message);
                    }

                    if (transportationRequest == null)
                    {
                        condition = $"{nameof(CustomerAddress.AddressType)} = '{TgAddressTypes.ShippingAddress.Value}'";
                        var condition2 = $"{nameof(CustomerAddress.CustomerId)} = {customer.Id}";
                        var condition3 = $"{nameof(CustomerAddress.IsDefault)} = 1";
                        var address =
                            await _customerAddressService.GetFirstOrDefaultAsync<CustomerAddress>(IsAdmin == false,
                                null, condition, condition2, condition3);
                        if (_customerAddressService.IsError)
                            return await ErrorResponse(_customerAddressService.Exception.Message);

                        string contact = null;
                        if (customer != null)
                        {
                            condition = $"{nameof(CustomerContactWithDetail.ContactTypeName)} IN ('Phone', 'Mobile')";
                            var customerContacts =
                                await _customerContactService.GetAllAsync<CustomerContactWithDetail>(
                                    IsAdmin == false, null, condition);
                            if (_customerContactService.IsError)
                                return await ErrorResponse(_customerContactService.Exception.Message);

                            contact = string.Join(",",
                                customerContacts.Select(c => $"{c.ContactTypeName}: {c.ContactData}"));
                        }

                        transportationRequest = new TransportationRequest
                        {
                            RequestNumber = 0,
                            RequestDate = DateTime.Now,
                            Status = TgOrderStatuses.New.Value,
                            StatusDate = DateTime.Now,
                            DueDate = customerOrder.DueDate,
                            Remark = $"Order #{customerOrder.OrderNumber:000000}" +
                                     (string.IsNullOrEmpty(customerOrder.CustomerReference)
                                         ? string.Empty
                                         : $", PO #{customerOrder.CustomerReference}"),
                            RecipientName = address?.Recipient ?? customer.Name,
                            Address = address?.Address ?? customer.Address,
                            SubDistrict = address?.SubDistrict ?? customer.SubDistrict,
                            District = address?.District ?? customer.District,
                            Province = address?.Province ?? customer.Province,
                            PostalCode = address?.PostalCode ?? customer.PostalCode,
                            Contact = contact
                        };

                        // Create transportation request reference
                        var transportationRequestReference = new TransportationRequestReference
                        {
                            InventoryRequestId = inventoryRequest.Id
                        };

                        transportationRequest =
                            await _transportationRequestService.InsertUpdateAsync(transportationRequest);
                        if (_transportationRequestService.IsError)
                            return await ErrorResponse(_transportationRequestService.Exception.Message);

                        transportationRequestReference.TransportationRequestId = transportationRequest.Id;
                        await _transportationRequestReferenceService.InsertUpdateAsync(transportationRequestReference);
                        if (_transportationRequestReferenceService.IsError)
                            return await ErrorResponse(_transportationRequestReferenceService.Exception.Message);
                    }

                    foreach (var customerOrderLine in customerOrderLines)
                    {
                        // if already generated
                        condition =
                            $"{nameof(CustomerOrderLineReference.CustomerOrderLineId)} = {customerOrderLine.Id}";
                        if (await _customerOrderLineReferenceService.IsExistsAsync<CustomerOrderLineReference>(
                            IsAdmin == false, null, condition)) continue;

                        var inventoryRequestLine = new InventoryRequestLine
                        {
                            InventoryRequestId = inventoryRequest.Id,
                            ProductId = customerOrderLine.ProductId,
                            Description = customerOrderLine.Description,
                            Quantity = customerOrderLine.Quantity,
                            Status = TgOrderStatuses.New.Value,
                            StatusDate = DateTime.Today
                        };

                        inventoryRequestLine =
                            await _inventoryRequestLineService.InsertUpdateAsync(inventoryRequestLine);
                        if (_inventoryRequestLineService.IsError)
                            return await ErrorResponse(_inventoryRequestLineService.Exception.Message);

                        // Create transportation request line
                        var transportationRequestLine = new TransportationRequestLine
                        {
                            TransportationRequestId = transportationRequest.Id,
                            ProductId = inventoryRequestLine?.ProductId ?? 0,
                            Quantity = inventoryRequestLine.Quantity
                        };

                        // Create reference
                        var transportationRequestLineReference = new TransportationRequestLineReference
                        {
                            InventoryRequestLineId = inventoryRequestLine.Id
                        };

                        transportationRequestLine =
                            await _transportationRequestLineService.InsertUpdateAsync(transportationRequestLine);
                        if (_transportationRequestLineService.IsError)
                            return await ErrorResponse(_transportationRequestLineService.Exception.Message);

                        transportationRequestLineReference.InventoryRequestLineId = transportationRequestLine.Id;
                        await _transportationRequestLineReferenceService.InsertUpdateAsync(
                            transportationRequestLineReference);
                        if (_transportationRequestLineReferenceService.IsError)
                            return await ErrorResponse(_transportationRequestLineReferenceService.Exception.Message);

                        // Set reference line
                        var customerOrderLineReference = new CustomerOrderLineReference
                        {
                            CustomerOrderLineId = customerOrderLine.Id,
                            InventoryRequestLineId = inventoryRequestLine.Id
                        };

                        await _customerOrderLineReferenceService.InsertUpdateAsync(customerOrderLineReference);
                        if (_customerOrderLineReferenceService.IsError)
                            return await ErrorResponse(_customerOrderLineReferenceService.Exception.Message);

                        // Add to booking reference
                        var stockBookingReference = new StockBookingReference
                        {
                            ProductId = customerOrderLine.ProductId,
                            InventoryRequestLineId = inventoryRequestLine.Id,
                            Quantity = customerOrderLine.Quantity,
                            DueDate = customerOrder.DueDate
                        };

                        await _stockBookingReferenceService.InsertUpdateAsync(stockBookingReference);
                        if (_stockBookingReferenceService.IsError)
                            return await ErrorResponse(_stockBookingReferenceService.Exception.Message);

                        // find current stock available
                        var productWithStock =
                            await _productService.GetAsync<ProductWithDetail>(customerOrderLine.ProductId,
                                IsAdmin == false);
                        if (_productService.IsError) return await ErrorResponse(_productService.Exception.Message);

                        // Available is included current booking quantity
                        if (productWithStock.AvailableQuantity >= 0m) continue;

                        var qty = productWithStock.AvailableQuantity * -1m;

                        var productionRequest = new ProductionRequest
                        {
                            RequestDate = DateTime.Today,
                            DueDate = customerOrder.DueDate < DateTime.Today ? DateTime.Today : customerOrder.DueDate,
                            Status = TgOrderStatuses.New.Value,
                            StatusDate = DateTime.Today,
                            ProductId = customerOrderLine.ProductId,
                            Quantity = qty,
                            Comment = $"{customer.Name} #{customerOrder.OrderNumber:000000}"
                        };

                        productionRequest = await _productionRequestService.InsertUpdateAsync(productionRequest);
                        if (_productionRequestService.IsError)
                            return await ErrorResponse(_productionRequestService.Exception.Message);

                        var productionRequestReference = new ProductionRequestReference
                        {
                            CustomerOrderId = customerOrder.Id,
                            ProductionRequestId = productionRequest.Id
                        };

                        await _productionRequestReferenceService.InsertUpdateAsync(productionRequestReference);
                        if (_productionRequestReferenceService.IsError)
                            return await ErrorResponse(_productionRequestReferenceService.Exception.Message);
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
        [ClaimRequired(RestrictItems.CustomerOrders, RestrictActions.Create)]
        public override async Task<IActionResult> InsertAsync([FromBody] CustomerOrder model)
        {
            return await base.InsertAsync(model);
        }

        [HttpPatch]
        [ClaimRequired(RestrictItems.CustomerOrders, RestrictActions.Update)]
        public override async Task<IActionResult> UpdateAsync([FromBody] CustomerOrder model)
        {
            return await base.UpdateAsync(model);
        }

        [HttpDelete]
        [ClaimRequired(RestrictItems.CustomerOrders, RestrictActions.Delete)]
        public override async Task<IActionResult> DeleteAsync([FromBody] int id)
        {
            return await base.DeleteAsync(id);
        }

        [HttpDelete("bulk-delete")]
        [ClaimRequired(RestrictItems.CustomerOrders, RestrictActions.Delete)]
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