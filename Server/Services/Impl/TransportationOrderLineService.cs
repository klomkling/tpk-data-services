using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class TransportationOrderLineService : ServiceBase<TransportationOrderLine>, ITransportationOrderLineService
    {
        public TransportationOrderLineService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<TransportationOrderLineService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
        }
    }
}