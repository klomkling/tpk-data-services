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
    [Route("api/production-requests")]
    public class ProductionRequestsController : TgControllerBase<ProductionRequestWithDetail, ProductionRequest>
    {
        private readonly IProductionRequestService _productionRequestService;
        private readonly IProductionOrderService _productionOrderService;
        private readonly IProductionOrderReferenceService _productionOrderReferenceService;

        public ProductionRequestsController(IServiceProvider serviceProvider,
            IProductionRequestService productionRequestService, IProductionOrderService productionOrderService,
            IProductionOrderReferenceService productionOrderReferenceService,
            ILogger<ProductionRequestsController> logger)
            : base(serviceProvider, productionRequestService, logger)
        {
            _productionRequestService = productionRequestService;
            _productionOrderService = productionOrderService;
            _productionOrderReferenceService = productionOrderReferenceService;
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
            try
            {
                var collection = await _productionRequestService.GetAllAsync<ProductionRequestWithDetail>(
                    IsAdmin == false, $"{nameof(ProductionRequestWithDetail.RequestNumber)} DESC");
                if (_productionRequestService.IsError)
                    return await ErrorResponse(_productionRequestService.Exception.Message);

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

        [HttpPost("generate-order")]
        [ClaimRequired(RestrictItems.ProductionRequests, RestrictActions.Create)]
        public async Task<IActionResult> GenerateProductionOrder([FromBody] IReadOnlyList<int> collection)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var listOfId = string.Join(",", collection.Select(id => id.ToString()));
                var condition = $"{nameof(ProductionRequest.Id)} IN ({listOfId})";
                var result =
                    await _productionRequestService.GetAllAsync<ProductionRequest>(IsAdmin == false, null,
                        condition);
                if (_productionRequestService.IsError)
                    return await ErrorResponse(_productionRequestService.Exception.Message);

                var productionRequests = result?.ToList() ?? new List<ProductionRequest>();
                if (productionRequests.Count == 0)
                    return await ErrorResponse("None of production request records is selected");

                var groupRequest = productionRequests.GroupBy(r => r.ProductId)
                    .Select(grp => new
                    {
                        ProductId = grp.Key,
                        Quantity = grp.Sum(r => r.Quantity),
                        DueDate = grp.Min(r => r.DueDate)
                    });

                foreach (var grpRequest in groupRequest)
                {
                    if (grpRequest.Quantity == 0m)
                        return await ErrorResponse($"Quantity for {grpRequest.ProductId} must bigger than zero");

                    var groupCollection = productionRequests.Where(r => r.ProductId == grpRequest.ProductId);

                    var productionOrder = new ProductionOrder
                    {
                        OrderDate = DateTime.Today,
                        DueDate = grpRequest.DueDate < DateTime.Today ? DateTime.Today : grpRequest.DueDate,
                        Status = TgOrderStatuses.New.Value,
                        StatusDate = DateTime.Today,
                        ProductId = grpRequest.ProductId,
                        LotNumber = "Waiting",
                        Quantity = grpRequest.Quantity,
                        Comment = "Customer order"
                    };

                    productionOrder = await _productionOrderService.InsertUpdateAsync(productionOrder);
                    if (_productionOrderService.IsError)
                        return await ErrorResponse(_productionOrderService.Exception.Message);

                    foreach (var productionRequest in groupCollection)
                    {
                        productionRequest.Status = TgOrderStatuses.Start.Value;
                        await _productionRequestService.InsertUpdateAsync(productionRequest);
                        if (_productionRequestService.IsError)
                            return await ErrorResponse(_productionRequestService.Exception.Message);

                        var productionOrderReference = new ProductionOrderReference
                        {
                            ProductionOrderId = productionOrder.Id,
                            ProductionRequestId = productionRequest.Id
                        };

                        await _productionOrderReferenceService.InsertUpdateAsync(productionOrderReference);
                        if (_productionOrderReferenceService.IsError == false) continue;

                        return await ErrorResponse(_productionOrderReferenceService.Exception.Message);
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
        [ClaimRequired(RestrictItems.ProductionRequests, RestrictActions.Create)]
        public override async Task<IActionResult> InsertAsync([FromBody] ProductionRequest model)
        {
            return await base.InsertAsync(model);
        }

        [HttpPatch]
        [ClaimRequired(RestrictItems.ProductionRequests, RestrictActions.Update)]
        public override async Task<IActionResult> UpdateAsync([FromBody] ProductionRequest model)
        {
            return await base.UpdateAsync(model);
        }

        [HttpDelete]
        [ClaimRequired(RestrictItems.ProductionRequests, RestrictActions.Delete)]
        public override async Task<IActionResult> DeleteAsync([FromBody] int id)
        {
            return await base.DeleteAsync(id);
        }

        [HttpDelete("bulk-delete")]
        [ClaimRequired(RestrictItems.ProductionRequests, RestrictActions.Delete)]
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