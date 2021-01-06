using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class ProductUnitService : ServiceBase<ProductUnit>, IProductUnitService
    {
        public ProductUnitService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<ProductUnitService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
            ExceptColumns = new List<string> {"IsPermanent"};
        }
    }
}