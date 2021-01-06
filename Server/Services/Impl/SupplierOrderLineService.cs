using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class SupplierOrderLineService : ServiceBase<SupplierOrderLine>, ISupplierOrderLineService
    {
        public SupplierOrderLineService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<ServiceBase<SupplierOrderLine>> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
        }
    }
}