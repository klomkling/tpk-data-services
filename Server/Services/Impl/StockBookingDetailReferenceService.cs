using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class StockBookingDetailReferenceService : ServiceBase<StockBookingDetailReference>,
        IStockBookingDetailReferenceService
    {
        public StockBookingDetailReferenceService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<StockBookingDetailReferenceService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
            IsKeepTracking = false;
            EnabledSoftDelete = false;
            AddEditProcedureName = "AddEditStockBookingDetailReference";
            DeleteProcedureName = "DeleteStockBookingDetailReference";
        }

        public int DeleteByStockId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(StockBookingDetailReference.StockId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByStockIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(StockBookingDetailReference.StockId));

            return DeleteAsync(id, parameters);
        }

        public int DeleteByInventoryRequestLineDetailId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(StockBookingDetailReference.InventoryRequestLineDetailId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByInventoryRequestLineDetailIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(StockBookingDetailReference.InventoryRequestLineDetailId));

            return DeleteAsync(id, parameters);
        }
    }
}