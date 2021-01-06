using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class SupplierContactService : ServiceBase<SupplierContact>, ISupplierContactService
    {
        public SupplierContactService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<SupplierContactService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
        }
    }
}