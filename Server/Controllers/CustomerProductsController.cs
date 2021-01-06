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
    [Route("api/customer-products")]
    public class CustomerProductsController : TgControllerBase<CustomerProductWithDetail, CustomerProduct>
    {
        private readonly ICustomerProductService _customerProductService;
        private readonly ICustomerProductPackageService _productPackageService;
        private readonly IStockroomService _stockroomService;
        private readonly IStockService _stockService;

        public CustomerProductsController(IServiceProvider serviceProvider,
            ICustomerProductService customerProductService, ICustomerProductPackageService productPackageService,
            IStockroomService stockroomService, IStockService stockService, ILogger<CustomerProductsController> logger)
            : base(serviceProvider, customerProductService, logger)
        {
            _customerProductService = customerProductService;
            _productPackageService = productPackageService;
            _stockroomService = stockroomService;
            _stockService = stockService;
        }

        [HttpGet("search")]
        public override Task<IActionResult> SearchAsync([FromQuery] string[] columns,
            [FromQuery] string[] searchStrings, [FromQuery] string[] orderColumns = null)
        {
            return base.SearchAsync(columns, searchStrings, orderColumns);
        }

        [HttpGet("{id:int}/packages")]
        public async Task<IActionResult> GetCustomerProductPackageAsync(int id,
            [FromQuery] DataSourceLoadOptions loadOptions)
        {
            try
            {
                var condition = $"{nameof(CustomerProductPackage.CustomerProductId)} = {id}";

                var collection =
                    await _productPackageService.GetAllAsync<CustomerProductPackageWithDetail>(IsAdmin == false, null,
                        condition);

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

        [HttpGet("{id:int}/stocks")]
        public async Task<IActionResult> GetCustomerProductStocksAsync(int id,
            [FromQuery] DataSourceLoadOptions loadOptions)
        {
            try
            {
                // Get customer product
                var customerProduct = await _customerProductService.GetAsync<CustomerProduct>(id);
                if (_customerProductService.IsError)
                    return await ErrorResponse(_customerProductService.Exception.Message);

                // Get customer product package
                var condition = $"{nameof(CustomerProductPackage.CustomerProductId)} = {id}";
                var packages =
                    await _productPackageService.GetAllAsync<CustomerProductPackage>(IsAdmin == false, null, condition);
                if (_productPackageService.IsError)
                    return await ErrorResponse(_productPackageService.Exception.Message);

                // Get stockroom
                var stockroom = await _stockroomService.GetByNameAsync("finished goods");
                if (_stockroomService.IsError) return await ErrorResponse(_stockroomService.Exception.Message);

                // Get stock
                condition = $"{nameof(StockWithDetail.StockroomId)} = {stockroom.Id}";
                var condition1 = $"{nameof(StockWithDetail.ProductId)} = {customerProduct.ProductId}";
                var condition2 =
                    $"{nameof(StockWithDetail.AvailableQuantity)} - {nameof(StockWithDetail.BookedQuantity)} > 0";
                var condition3 = string.Empty;
                var packageCollection = packages?.ToList() ?? new List<CustomerProductPackage>();
                if (packageCollection.Count > 0)
                {
                    var listOfId = string.Join(",", packageCollection.Select(p => p.PackageTypeId.ToString()));
                    condition3 = $"{nameof(StockWithDetail.PackageTypeId)} IN ({listOfId})";
                }

                var orderClause = $"{nameof(StockWithDetail.ReceivedDate)}, {nameof(StockWithDetail.PackageCode)}, " +
                                  $"{nameof(StockWithDetail.LotNumber)}, {nameof(StockWithDetail.PalletNo)}, " +
                                  $"{nameof(StockWithDetail.PackageNumber)}";

                var stocks = await _stockService.GetAllAsync<StockWithDetail>(IsAdmin == false, orderClause,
                    condition, condition1, condition2, condition3);
                if (_stockService.IsError) return await ErrorResponse(_stockService.Exception.Message);

                var result = await Task.Run(() => DataSourceLoader.Load(stocks.AsQueryable(), loadOptions));
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
        [ClaimRequired(RestrictItems.CustomerProducts, RestrictActions.Create)]
        public override async Task<IActionResult> InsertAsync([FromBody] CustomerProduct model)
        {
            return await base.InsertAsync(model);
        }

        [HttpPatch]
        [ClaimRequired(RestrictItems.CustomerProducts, RestrictActions.Update)]
        public override async Task<IActionResult> UpdateAsync([FromBody] CustomerProduct model)
        {
            return await base.UpdateAsync(model);
        }

        [HttpDelete]
        [ClaimRequired(RestrictItems.CustomerProducts, RestrictActions.Delete)]
        public override async Task<IActionResult> DeleteAsync([FromBody] int id)
        {
            return await base.DeleteAsync(id);
        }

        [HttpDelete("bulk-delete")]
        [ClaimRequired(RestrictItems.CustomerProducts, RestrictActions.Delete)]
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