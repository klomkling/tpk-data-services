using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class SupplierOrderService : ServiceBase<SupplierOrder>, ISupplierOrderService
    {
        public SupplierOrderService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ISupplierOrderLineService supplierOrderLineService,
            ILogger<SupplierOrderService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
        }
    }
}