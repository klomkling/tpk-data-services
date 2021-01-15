using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tpk.DataServices.Server.Classes.Impl;
using Tpk.DataServices.Server.Services;
using Tpk.DataServices.Shared.Data.Constants;
using Tpk.DataServices.Shared.Data.Views;

namespace Tpk.DataServices.Server.Controllers
{
    [AuthorizeRequired(RestrictRoles.Administrator, RestrictRoles.Director)]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICustomerOrderService _customerOrderService;
        private ILogger<CompaniesController> _logger;

        public AdminController(IServiceProvider serviceProvider, ICustomerOrderService customerOrderService,
            ILogger<CompaniesController> logger)
        {
            var roleService = (IRoleService) serviceProvider.GetService(typeof(IRoleService));
            _serviceProvider = serviceProvider;
            _customerOrderService = customerOrderService;
            _logger = logger;
            IsAdmin = roleService?.IsAdmin(customerOrderService.CurrentUser) ?? false;
        }

        public bool IsAdmin { get; set; }

        [HttpPost("first-import")]
        public async Task<IActionResult> FirstImport()
        {
            var customerOrders = await _customerOrderService.GetAllAsync<CustomerOrderWithDetail>();
            var collection = customerOrders.Where(o => o.CanGenerate).Select(o => o.Id).ToList();
            var customerOrderHelper = new CustomerOrderHelpers(_serviceProvider);
            if (await customerOrderHelper.GenerateInventoryRequestAsync(collection))
            {
                return Ok(true);
            }

            if (string.IsNullOrEmpty(customerOrderHelper.ErrorMessage) == false)
            {
                _logger.LogError(customerOrderHelper.ErrorMessage);
                return Ok(false);
            }
            
            _logger.LogError(customerOrderHelper.ErrorMessage);
            return await Task.FromResult(StatusCode(StatusCodes.Status500InternalServerError,
                new {message = customerOrderHelper.ErrorMessage}));
        }
    }
}