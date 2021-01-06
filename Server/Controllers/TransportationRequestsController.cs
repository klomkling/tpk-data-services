using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Route("api/transportation-requests")]
    public class
        TransportationRequestsController : TgControllerBase<TransportationRequestWithDetail, TransportationRequest>
    {
        private readonly ITransportationRequestService _transportationRequestService;
        private readonly ITransportationRequestScheduleService _transportationRequestScheduleService;

        public TransportationRequestsController(IServiceProvider serviceProvider,
            ITransportationRequestService transportationRequestService,
            ITransportationRequestScheduleService transportationRequestScheduleService,
            ILogger<TransportationRequestsController> logger)
            : base(serviceProvider, transportationRequestService, logger)
        {
            _transportationRequestService = transportationRequestService;
            _transportationRequestScheduleService = transportationRequestScheduleService;
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
                // var condition =
                //     $"{nameof(TransportationRequestSchedule.StartDate)} >= '{DateTime.Today.FirstDayOfWeek():yyyy-MM-dd}'";
                var condition = $"{nameof(TransportationRequestSchedule.StartDate)} >= '{DateTime.Today:yyyy-MM-dd}'";
                var collection =
                    await _transportationRequestScheduleService.GetAllAsync<TransportationRequestSchedule>(
                        false, "StartDate", condition);

                if (_transportationRequestScheduleService.IsError)
                    return await ErrorResponse(_transportationRequestScheduleService.Exception.Message);

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
                var collection = await _transportationRequestService.GetAllAsync<TransportationRequestWithDetail>(
                    IsAdmin == false, $"{nameof(TransportationRequestWithDetail.RequestNumber)} DESC");
                if (_transportationRequestService.IsError)
                    return await ErrorResponse(_transportationRequestService.Exception.Message);

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
        [ClaimRequired(RestrictItems.ProductColors, RestrictActions.Create)]
        public override async Task<IActionResult> InsertAsync([FromBody] TransportationRequest model)
        {
            return await base.InsertAsync(model);
        }

        [HttpPatch]
        [ClaimRequired(RestrictItems.ProductColors, RestrictActions.Update)]
        public override async Task<IActionResult> UpdateAsync([FromBody] TransportationRequest model)
        {
            return await base.UpdateAsync(model);
        }

        [HttpDelete]
        [ClaimRequired(RestrictItems.ProductColors, RestrictActions.Delete)]
        public override async Task<IActionResult> DeleteAsync([FromBody] int id)
        {
            return await base.DeleteAsync(id);
        }

        [HttpDelete("bulk-delete")]
        [ClaimRequired(RestrictItems.ProductColors, RestrictActions.Delete)]
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