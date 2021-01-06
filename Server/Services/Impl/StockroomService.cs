using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class StockroomService : ServiceBase<Stockroom>, IStockroomService
    {
        public StockroomService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<StockroomService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
            ExceptColumns = new List<string> {"IsPermanent"};
        }

        public Stockroom GetByName(string name, bool isActive = true)
        {
            var result = GetFirstOrDefault<Stockroom>(isActive, null, $"{nameof(Stockroom.Name)} = '{name}'");
            return IsError ? null : result;
        }

        public async Task<Stockroom> GetByNameAsync(string name, bool isActive = true)
        {
            var result =
                await GetFirstOrDefaultAsync<Stockroom>(isActive, null, $"{nameof(Stockroom.Name)} = '{name}'");
            return IsError ? null : result;
        }
    }
}