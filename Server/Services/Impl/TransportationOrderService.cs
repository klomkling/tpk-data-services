using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class TransportationOrderService : ServiceBase<TransportationOrder>, ITransportationOrderService
    {
        public TransportationOrderService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<TransportationOrderService> logger) 
            : base(configuration, options, httpContextAccessor, logger)
        {
        }
    }
}