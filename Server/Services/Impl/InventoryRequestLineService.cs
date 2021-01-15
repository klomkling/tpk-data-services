using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class InventoryRequestLineService : ServiceBase<InventoryRequestLine>, IInventoryRequestLineService
    {
        public InventoryRequestLineService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<InventoryRequestLineService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
            EnabledSoftDelete = false;
        }
    }
}