using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class StockBookingReferenceService : ServiceBase<StockBookingReference>, IStockBookingReferenceService
    {
        public StockBookingReferenceService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<StockBookingReferenceService> logger) : base(
            configuration, options, httpContextAccessor, logger)
        {
            IsKeepTracking = false;
            EnabledSoftDelete = false;
            AddEditProcedureName = "AddEditStockBookingReference";
            DeleteProcedureName = "DeleteStockBookingReference";
        }

        public int DeleteByProductId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(StockBookingReference.ProductId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByProductIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(StockBookingReference.ProductId));

            return DeleteAsync(id, parameters);
        }

        public int DeleteByInventoryRequestLineId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(StockBookingReference.InventoryRequestLineId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByInventoryRequestLineIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(StockBookingReference.InventoryRequestLineId));

            return DeleteAsync(id, parameters);
        }
    }
}