using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class CustomerOrderLineService : ServiceBase<CustomerOrderLine>, ICustomerOrderLineService
    {
        public CustomerOrderLineService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<CustomerOrderLineService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
        }
    }
}