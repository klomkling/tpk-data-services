using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class InventoryRequestLineDetailService : ServiceBase<InventoryRequestLineDetail>,
        IInventoryRequestLineDetailService
    {
        public InventoryRequestLineDetailService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<InventoryRequestLineDetailService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
            EnabledSoftDelete = false;
        }
    }
}