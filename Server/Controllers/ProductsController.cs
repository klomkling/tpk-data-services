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
    public class ProductsController : TgControllerBase<ProductWithDetail, Product>
    {
        private readonly IProductPackageService _productPackagingService;
        private readonly IStockService _stockService;
        private readonly IStockBookingReferenceService _stockBookingReferenceService;
        private readonly IProductService _productService;

        public ProductsController(IServiceProvider serviceProvider, IProductService productService,
            IProductPackageService productPackagingService, IStockService stockService,
            IStockBookingReferenceService stockBookingReferenceService,
            ILogger<ProductsController> logger)
            : base(serviceProvider, productService, logger)
        {
            _productService = productService;
            _productPackagingService = productPackagingService;
            _stockService = stockService;
            _stockBookingReferenceService = stockBookingReferenceService;
        }

        [HttpGet("search")]
        public override Task<IActionResult> SearchAsync([FromQuery] string[] columns,
            [FromQuery] string[] searchStrings, [FromQuery] string[] orderColumns = null)
        {
            return base.SearchAsync(columns, searchStrings, orderColumns);
        }

        [HttpGet("code")]
        public async Task<IActionResult> GetByCode([FromQuery] string code)
        {
            try
            {
                var result = await _productService.GetFirstOrDefaultAsync<Product>(IsAdmin == false, null,
                    $"{nameof(Product.Code)} = '{code}'");

                if (_productService.IsError) return await ErrorResponse(_productService.Exception.Message);

                return Ok(result);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpGet("{id:int}/packages")]
        public async Task<IActionResult> GetProductPackagingsAsync(int id,
            [FromQuery] DataSourceLoadOptions loadOptions)
        {
            try
            {
                var condition = $"{nameof(ProductPackage.ProductId)} = {id}";

                var collection =
                    await _productPackagingService.GetAllAsync<ProductPackageWithDetail>(IsAdmin == false, null,
                        condition);

                if (_productPackagingService.IsError)
                    return await ErrorResponse(_productPackagingService.Exception.Message);

                var result = await Task.Run(() => DataSourceLoader.Load(collection.AsQueryable(), loadOptions));
                return Ok(result);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpGet("{id:int}/stocks")]
        public async Task<IActionResult> GetStockByProductIdAsync(int id, [FromQuery] DataSourceLoadOptions loadOptions)
        {
            var condition1 = $"{nameof(StockWithDetail.ProductId)} = {id}";
            var condition2 =
                $"{nameof(StockWithDetail.AvailableQuantity)} - {nameof(StockWithDetail.BookedQuantity)} > 0";
            // var orderClause = $"{nameof(StockWithDetail.ProductCode)}, {nameof(StockWithDetail.StockroomName)}, " +
            //                   $"{nameof(StockWithDetail.StockLocation)}, {nameof(StockWithDetail.PackageCode)}, " +
            //                   $"{nameof(StockWithDetail.ReceivedDate)}";
            var orderClause =
                $"CAST ({nameof(StockWithDetail.ReceivedDate)} AS DATE), {nameof(StockWithDetail.PackageCode)}, " +
                $"{nameof(StockWithDetail.PalletNo)}, {nameof(StockWithDetail.PackageNumber)}, " +
                $"{nameof(StockWithDetail.StockroomName)}, {nameof(StockWithDetail.StockLocation)}";
            
            var collection =
                await _stockService.GetAllAsync<StockWithDetail>(IsAdmin == false, orderClause,
                    condition1, condition2);

            if (_stockService.IsError) return await ErrorResponse(_stockService.Exception.Message);

            var result = await Task.Run(() => DataSourceLoader.Load(collection.AsQueryable(), loadOptions));
            return Ok(result);
        }

        [HttpGet("{id:int}/bookings")]
        public async Task<IActionResult> GetProductBookingsAsync(int id, [FromQuery] DataSourceLoadOptions loadOptions)
        {
            try
            {
                var condition = $"{nameof(StockBookingReferenceWithDetail.ProductId)} = {id}";
                var orderClause = $"{nameof(StockBookingReferenceWithDetail.DueDate)}, " +
                                  $"{nameof(StockBookingReferenceWithDetail.CustomerName)}, " +
                                  $"{nameof(StockBookingReferenceWithDetail.OrderNumber)}";
                var collection =
                    await _stockBookingReferenceService.GetAllAsync<StockBookingReferenceWithDetail>(
                        false, orderClause, condition);

                if (_stockBookingReferenceService.IsError)
                    return await ErrorResponse(_stockBookingReferenceService.Exception.Message);

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
            var collection =
                await _productService.GetAllAsync<ProductWithDetail>(IsAdmin == false, null);

            if (_productService.IsError) return await ErrorResponse(_productService.Exception.Message);

            var result = await Task.Run(() => DataSourceLoader.Load(collection.AsQueryable(), loadOptions));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public override async Task<IActionResult> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        [HttpPost]
        [ClaimRequired(RestrictItems.Products, RestrictActions.Create)]
        public override async Task<IActionResult> InsertAsync([FromBody] Product model)
        {
            return await base.InsertAsync(model);
        }

        [HttpPatch]
        [ClaimRequired(RestrictItems.Products, RestrictActions.Update)]
        public override async Task<IActionResult> UpdateAsync([FromBody] Product model)
        {
            return await base.UpdateAsync(model);
        }

        [HttpDelete]
        [ClaimRequired(RestrictItems.Products, RestrictActions.Delete)]
        public override async Task<IActionResult> DeleteAsync([FromBody] int id)
        {
            return await base.DeleteAsync(id);
        }

        [HttpDelete("bulk-delete")]
        [ClaimRequired(RestrictItems.Products, RestrictActions.Delete)]
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