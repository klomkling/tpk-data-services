using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Tpk.DataServices.Server.Services;
using Tpk.DataServices.Shared.Data.Models;
using Tpk.DataServices.Shared.Data.Views;

namespace Tpk.DataServices.Server.Classes.Impl
{
    public class CustomerOrderHelpers
    {
        private readonly ICustomerOrderService _customerOrderService;
        private readonly ICustomerOrderReferenceService _customerOrderReferenceService;
        private readonly ICustomerOrderLineService _customerOrderLineService;
        private readonly IInventoryRequestService _inventoryRequestService;
        private readonly IStockroomService _stockroomService;
        private readonly ITransportationRequestReferenceService _transportationRequestReferenceService;
        private readonly ITransportationRequestService _transportationRequestService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerAddressService _customerAddressService;
        private readonly ICustomerContactService _customerContactService;
        private readonly ICustomerOrderLineReferenceService _customerOrderLineReferenceService;
        private readonly IInventoryRequestLineService _inventoryRequestLineService;
        private readonly ITransportationRequestLineService _transportationRequestLineService;
        private readonly ITransportationRequestLineReferenceService _transportationRequestLineReferenceService;
        private readonly IStockBookingReferenceService _stockBookingReferenceService;
        private readonly IProductService _productService;
        private readonly IProductionRequestService _productionRequestService;
        private readonly IProductionRequestReferenceService _productionRequestReferenceService;

        public CustomerOrderHelpers(IServiceProvider serviceProvider)
        {
            var roleService = (IRoleService) serviceProvider.GetService(typeof(IRoleService));
            _customerService = (ICustomerService) serviceProvider.GetService(typeof(ICustomerService));
            _customerAddressService = (ICustomerAddressService) serviceProvider
                .GetService(typeof(ICustomerAddressService));
            _customerContactService = (ICustomerContactService) serviceProvider
                .GetService(typeof(ICustomerContactService));
            _customerOrderService = (ICustomerOrderService) serviceProvider
                .GetService(typeof(ICustomerOrderService));
            _customerOrderReferenceService = (ICustomerOrderReferenceService) serviceProvider
                .GetService(typeof(ICustomerOrderReferenceService));
            _customerOrderLineService = (ICustomerOrderLineService) serviceProvider
                .GetService(typeof(ICustomerOrderLineService));
            _customerOrderLineReferenceService = (ICustomerOrderLineReferenceService) serviceProvider
                .GetService(typeof(ICustomerOrderLineReferenceService));
            _inventoryRequestService = (IInventoryRequestService) serviceProvider
                .GetService(typeof(IInventoryRequestService));
            _inventoryRequestLineService = (IInventoryRequestLineService) serviceProvider
                .GetService(typeof(IInventoryRequestLineService));
            _transportationRequestService = (ITransportationRequestService) serviceProvider
                .GetService(typeof(ITransportationRequestService));
            _transportationRequestReferenceService = (ITransportationRequestReferenceService) serviceProvider
                .GetService(typeof(ITransportationRequestReferenceService));
            _transportationRequestLineService = (ITransportationRequestLineService) serviceProvider
                .GetService(typeof(ITransportationRequestLineService));
            _transportationRequestLineReferenceService = (ITransportationRequestLineReferenceService) serviceProvider
                .GetService(typeof(ITransportationRequestLineReferenceService));
            _stockroomService = (IStockroomService) serviceProvider.GetService(typeof(IStockroomService));
            _stockBookingReferenceService = (IStockBookingReferenceService) serviceProvider
                .GetService(typeof(IStockBookingReferenceService));
            _productService = (IProductService) serviceProvider.GetService(typeof(IProductService));
            _productionRequestService = (IProductionRequestService) serviceProvider
                .GetService(typeof(IProductionRequestService));
            _productionRequestReferenceService = (IProductionRequestReferenceService) serviceProvider
                .GetService(typeof(IProductionRequestReferenceService));

            IsAdmin = roleService.IsAdmin(_customerOrderService.CurrentUser);
        }

        public bool IsAdmin { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<bool> GenerateInventoryRequestAsync(IReadOnlyList<int> collection)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var listOfId = string.Join(",", collection.Select(id => id.ToString()));
                var condition = $"{nameof(CustomerOrderWithDetail.Id)} IN ({listOfId})";
                var result =
                    await _customerOrderService.GetAllAsync<CustomerOrderWithDetail>(IsAdmin == false, null, condition);
                if (_customerOrderService.IsError)
                {
                    ErrorMessage = _customerOrderService.Exception.Message;
                    return false;
                }

                var customerOrders = result?.ToList() ?? new List<CustomerOrderWithDetail>();
                if (customerOrders.Count == 0)
                {
                    ErrorMessage = "None of customer order is selected";
                    return false;
                }

                var stockroom = await _stockroomService.GetByNameAsync("stock out");
                if (_stockroomService.IsError)
                {
                    ErrorMessage = "Cannot find stockroom";
                    return false;
                }

                var generatedCount = 0;
                foreach (var customerOrder in customerOrders)
                {
                    if (customerOrder.CanGenerate == false) continue;

                    condition = $"{nameof(CustomerOrderLineWithDetail.CustomerOrderId)} = {customerOrder.Id}";
                    var lineResult =
                        await _customerOrderLineService.GetAllAsync<CustomerOrderLineWithDetail>(IsAdmin == false, null,
                            condition);
                    if (_customerOrderLineService.IsError)
                    {
                        ErrorMessage = _customerOrderLineService.Exception.Message;
                        return false;
                    }

                    var customerOrderLines = lineResult?.ToList() ?? new List<CustomerOrderLineWithDetail>();
                    if (customerOrderLines.Count == 0)
                    {
                        ErrorMessage = "None of customer order lines is available";
                        return false;
                    }

                    var customerOrderReference =
                        await _customerOrderReferenceService.GetAsync<CustomerOrderReference>(customerOrder.Id, false);
                    if (_customerOrderReferenceService.IsError)
                    {
                        ErrorMessage = _customerOrderReferenceService.Exception.Message;
                        return false;
                    }

                    InventoryRequest inventoryRequest;
                    TransportationRequest transportationRequest;
                    if (customerOrderReference == null)
                    {
                        inventoryRequest = null;
                        transportationRequest = null;
                    }
                    else
                    {
                        inventoryRequest = await _inventoryRequestService
                            .GetAsync<InventoryRequest>(customerOrderReference.InventoryRequestId, IsAdmin == false);
                        if (_inventoryRequestService.IsError)
                        {
                            ErrorMessage = _inventoryRequestService.Exception.Message;
                            return false;
                        }

                        condition =
                            $"{nameof(TransportationRequestReference.InventoryRequestId)} = {inventoryRequest.Id}";
                        var transportationRequestReference = await _transportationRequestReferenceService
                            .GetFirstOrDefaultAsync<TransportationRequestReference>(IsAdmin == false, condition);
                        if (_transportationRequestReferenceService.IsError)
                        {
                            ErrorMessage = _transportationRequestReferenceService.Exception.Message;
                            return false;
                        }

                        if (transportationRequestReference != null)
                        {
                            transportationRequest =
                                await _transportationRequestService.GetAsync<TransportationRequest>(
                                    transportationRequestReference.InventoryRequestId);
                            if (_transportationRequestService.IsError)
                            {
                                ErrorMessage = _transportationRequestService.Exception.Message;
                                return false;
                            }
                        }
                        else
                        {
                            transportationRequest = null;
                        }
                    }

                    var customer = await _customerService.GetAsync<Customer>(customerOrder.CustomerId);
                    if (_customerService.IsError)
                    {
                        ErrorMessage = _customerService.Exception.Message;
                        return false;
                    }

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
                        {
                            ErrorMessage = _inventoryRequestService.Exception.Message;
                            return false;
                        }

                        customerOrderReference = new CustomerOrderReference
                        {
                            CustomerOrderId = customerOrder.Id,
                            InventoryRequestId = inventoryRequest.Id
                        };

                        await _customerOrderReferenceService.InsertUpdateAsync(customerOrderReference);
                        if (_customerOrderReferenceService.IsError)
                        {
                            ErrorMessage = _customerOrderReferenceService.Exception.Message;
                            return false;
                        }
                    }

                    if (transportationRequest == null)
                    {
                        condition = $"{nameof(CustomerAddress.AddressType)} = '{TgAddressTypes.ShippingAddress.Value}'";
                        var condition2 = $"{nameof(CustomerAddress.CustomerId)} = {customer.Id}";
                        var condition3 = $"{nameof(CustomerAddress.IsDefault)} = 1";
                        var address = await _customerAddressService
                            .GetFirstOrDefaultAsync<CustomerAddress>(IsAdmin == false, null,
                                condition, condition2, condition3);
                        if (_customerAddressService.IsError)
                        {
                            ErrorMessage = _customerAddressService.Exception.Message;
                            return false;
                        }

                        string contact = null;
                        if (customer != null)
                        {
                            condition = $"{nameof(CustomerContactWithDetail.ContactTypeName)} IN ('Phone', 'Mobile')";
                            var customerContacts = await _customerContactService
                                .GetAllAsync<CustomerContactWithDetail>(IsAdmin == false, null, condition);
                            if (_customerContactService.IsError)
                            {
                                ErrorMessage = _customerContactService.Exception.Message;
                                return false;
                            }

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

                        transportationRequest = await _transportationRequestService
                            .InsertUpdateAsync(transportationRequest);
                        if (_transportationRequestService.IsError)
                        {
                            ErrorMessage = _transportationRequestService.Exception.Message;
                            return false;
                        }

                        transportationRequestReference.TransportationRequestId = transportationRequest.Id;
                        await _transportationRequestReferenceService.InsertUpdateAsync(transportationRequestReference);
                        if (_transportationRequestReferenceService.IsError)
                        {
                            ErrorMessage = _transportationRequestReferenceService.Exception.Message;
                            return false;
                        }
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

                        inventoryRequestLine = await _inventoryRequestLineService.InsertUpdateAsync(inventoryRequestLine);
                        if (_inventoryRequestLineService.IsError)
                        {
                            ErrorMessage = _inventoryRequestLineService.Exception.Message;
                            return false;
                        }

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

                        transportationRequestLine = await _transportationRequestLineService
                            .InsertUpdateAsync(transportationRequestLine);
                        if (_transportationRequestLineService.IsError)
                        {
                            ErrorMessage = _transportationRequestLineService.Exception.Message;
                            return false;
                        }

                        transportationRequestLineReference.InventoryRequestLineId = transportationRequestLine.Id;
                        await _transportationRequestLineReferenceService
                            .InsertUpdateAsync(transportationRequestLineReference);
                        if (_transportationRequestLineReferenceService.IsError)
                        {
                            ErrorMessage = _transportationRequestLineReferenceService.Exception.Message;
                            return false;
                        }

                        // Set reference line
                        var customerOrderLineReference = new CustomerOrderLineReference
                        {
                            CustomerOrderLineId = customerOrderLine.Id,
                            InventoryRequestLineId = inventoryRequestLine.Id
                        };

                        await _customerOrderLineReferenceService.InsertUpdateAsync(customerOrderLineReference);
                        if (_customerOrderLineReferenceService.IsError)
                        {
                            ErrorMessage = _customerOrderLineReferenceService.Exception.Message;
                            return false;
                        }

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
                        {
                            ErrorMessage = _stockBookingReferenceService.Exception.Message;
                            return false;
                        }

                        // find current stock available
                        var productWithStock = await _productService
                            .GetAsync<ProductWithDetail>(customerOrderLine.ProductId, IsAdmin == false);
                        if (_productService.IsError)
                        {
                            ErrorMessage = _productService.Exception.Message;
                            return false;
                        }

                        // Available is included current booking quantity
                        if (productWithStock.AvailableQuantity - customerOrderLine.Quantity >= 0m) continue;

                        //var qty = productWithStock.AvailableQuantity * -1m;
                        var qty = productWithStock.AvailableQuantity > 0
                            ? customerOrderLine.Quantity - productWithStock.AvailableQuantity
                            : customerOrderLine.Quantity;

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
                        {
                            ErrorMessage = _productionRequestService.Exception.Message;
                            return false;
                        }

                        var productionRequestReference = new ProductionRequestReference
                        {
                            CustomerOrderId = customerOrder.Id,
                            ProductionRequestId = productionRequest.Id
                        };

                        await _productionRequestReferenceService.InsertUpdateAsync(productionRequestReference);
                        if (!_productionRequestReferenceService.IsError) continue;
                        
                        ErrorMessage = _productionRequestReferenceService.Exception.Message;
                        return false;
                    }

                    
                    generatedCount++;
                }

                if (generatedCount == 0) return false;

                scope.Complete();
                return true;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return false;
            }
        }
    }
}