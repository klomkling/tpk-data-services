using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class ProductCategoryService : ServiceBase<ProductCategory>, IProductCategoryService
    {
        public ProductCategoryService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<ProductCategoryService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
        }
    }
}