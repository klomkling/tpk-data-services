using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tpk.DataServices.Server.Classes.Impl;
using Tpk.DataServices.Server.Services;
using Tpk.DataServices.Shared.Data.Constants;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Controllers
{
    [AuthorizeRequired]
    [ApiController]
    [Route("api/stock-locations")]
    public class StockLocationsController : TgControllerBase<StockLocation, StockLocation>
    {
        public StockLocationsController(IServiceProvider serviceProvider, IStockLocationService stockLocationService,
            ILogger<StockLocationsController> logger)
            : base(serviceProvider, stockLocationService, logger)
        {
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
            return await base.GetAllAsync(loadOptions);
        }

        [HttpGet("{id:int}")]
        public override async Task<IActionResult> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        [HttpPost]
        [ClaimRequired(RestrictItems.StockLocations, RestrictActions.Create)]
        public override async Task<IActionResult> InsertAsync([FromBody] StockLocation model)
        {
            return await base.InsertAsync(model);
        }

        [HttpPatch]
        [ClaimRequired(RestrictItems.StockLocations, RestrictActions.Update)]
        public override async Task<IActionResult> UpdateAsync([FromBody] StockLocation model)
        {
            return await base.UpdateAsync(model);
        }

        [HttpDelete]
        [ClaimRequired(RestrictItems.StockLocations, RestrictActions.Delete)]
        public override async Task<IActionResult> DeleteAsync([FromBody] int id)
        {
            return await base.DeleteAsync(id);
        }

        [HttpDelete("bulk-delete")]
        [ClaimRequired(RestrictItems.StockLocations, RestrictActions.Delete)]
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