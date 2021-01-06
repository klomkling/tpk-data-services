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
    public class CustomersController : TgControllerBase<Customer, Customer>
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerContactService _customerContactService;
        private readonly ICustomerProductService _customerProductService;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly ICustomerAddressService _customerAddressService;

        public CustomersController(IServiceProvider serviceProvider, ICustomerService customerService,
            ICustomerContactService customerContactService, ICustomerProductService customerProductService,
            ICustomerOrderService customerOrderService, ICustomerAddressService customerAddressService,
            ILogger<CustomersController> logger)
            : base(serviceProvider, customerService, logger)
        {
            _customerService = customerService;
            _customerContactService = customerContactService;
            _customerProductService = customerProductService;
            _customerOrderService = customerOrderService;
            _customerAddressService = customerAddressService;
        }

        [HttpGet("search")]
        public override Task<IActionResult> SearchAsync([FromQuery] string[] columns,
            [FromQuery] string[] searchStrings, [FromQuery] string[] orderColumns = null)
        {
            return base.SearchAsync(columns, searchStrings, orderColumns);
        }

        [HttpGet("{id:int}/contacts")]
        public async Task<IActionResult> GetCustomerContactsAsync(int id, [FromQuery] DataSourceLoadOptions loadOptions)
        {
            try
            {
                var condition = $"{nameof(CustomerContact.CustomerId)} = {id}";

                var collection =
                    await _customerContactService.GetAllAsync<CustomerContactWithDetail>(IsAdmin == false, null,
                        condition);

                if (_customerContactService.IsError)
                    return await ErrorResponse(_customerContactService.Exception.Message);

                var result = await Task.Run(() => DataSourceLoader.Load(collection.AsQueryable(), loadOptions));
                return Ok(result);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpGet("{id:int}/addresses")]
        public async Task<IActionResult> GetCustomerAddressesAsync(int id,
            [FromQuery] DataSourceLoadOptions loadOptions)
        {
            try
            {
                var condition = $"{nameof(CustomerAddress.CustomerId)} = {id}";

                var collection =
                    await _customerAddressService.GetAllAsync<CustomerAddress>(IsAdmin == false, null, condition);

                if (_customerAddressService.IsError)
                    return await ErrorResponse(_customerAddressService.Exception.Message);

                var result = await Task.Run(() => DataSourceLoader.Load(collection.AsQueryable(), loadOptions));
                return Ok(result);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpGet("{id:int}/products")]
        public async Task<IActionResult> GetCustomerProductsAsync(int id, [FromQuery] DataSourceLoadOptions loadOptions)
        {
            try
            {
                var condition = $"{nameof(CustomerProductWithDetail.CustomerId)} = {id}";

                var collection =
                    await _customerProductService.GetAllAsync<CustomerProductWithDetail>(IsAdmin == false, null,
                        condition);

                if (_customerContactService.IsError)
                    return await ErrorResponse(_customerProductService.Exception.Message);

                var result = await Task.Run(() => DataSourceLoader.Load(collection.AsQueryable(), loadOptions));
                return Ok(result);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpGet("{id:int}/orders")]
        public async Task<IActionResult> GetCustomerOrdersAsync(int id, [FromQuery] DataSourceLoadOptions loadOptions)
        {
            try
            {
                var condition = $"{nameof(CustomerOrder.CustomerId)} = {id}";

                var collection =
                    await _customerOrderService.GetAllAsync<CustomerOrderWithDetail>(IsAdmin == false,
                        $"{nameof(CustomerOrderWithDetail.OrderNumber)} DESC", condition);

                if (_customerOrderService.IsError)
                    return await ErrorResponse(_customerOrderService.Exception.Message);

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
            try
            {
                var collection = await _customerService.GetAllAsync<CustomerWithDetail>(IsAdmin == false, "Code");
                if (_customerService.IsError) return await ErrorResponse(_customerService.Exception.Message);

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
        [ClaimRequired(RestrictItems.Customers, RestrictActions.Create)]
        public override async Task<IActionResult> InsertAsync([FromBody] Customer model)
        {
            return await base.InsertAsync(model);
        }

        [HttpPatch]
        [ClaimRequired(RestrictItems.Customers, RestrictActions.Update)]
        public override async Task<IActionResult> UpdateAsync([FromBody] Customer model)
        {
            return await base.UpdateAsync(model);
        }

        [HttpDelete]
        [ClaimRequired(RestrictItems.Customers, RestrictActions.Delete)]
        public override async Task<IActionResult> DeleteAsync([FromBody] int id)
        {
            return await base.DeleteAsync(id);
        }

        [HttpDelete("bulk-delete")]
        [ClaimRequired(RestrictItems.Customers, RestrictActions.Delete)]
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