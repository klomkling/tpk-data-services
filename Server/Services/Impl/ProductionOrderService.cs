using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class ProductionOrderService : ServiceBase<ProductionOrder>, IProductionOrderService
    {
        public ProductionOrderService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<ProductionOrderService> logger) 
            : base(configuration, options, httpContextAccessor, logger)
        {
        }
    }
}