using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class UnitService : ServiceBase<Unit>, IUnitService
    {
        public UnitService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<UnitService> logger) 
            : base(configuration, options, httpContextAccessor, logger)
        {
        }
    }
}