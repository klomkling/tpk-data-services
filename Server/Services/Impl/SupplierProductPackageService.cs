using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class SupplierProductPackageService : ServiceBase<SupplierProductPackage>, ISupplierProductPackageService
    {
        public SupplierProductPackageService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<SupplierProductPackageService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
        }
    }
}