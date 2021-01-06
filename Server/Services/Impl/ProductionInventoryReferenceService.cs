using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class ProductionInventoryReferenceService : ServiceBase<ProductionInventoryReference>,
        IProductionInventoryReferenceService
    {
        public ProductionInventoryReferenceService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<ProductionInventoryReferenceService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
            IsKeepTracking = false;
            EnabledSoftDelete = false;
            AddEditProcedureName = "AddEditProductionInventoryReference";
            DeleteProcedureName = "DeleteProductionInventoryReference";
        }

        public int DeleteByProductionOrderId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(ProductionInventoryReference.ProductionOrderId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByProductionOrderIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(ProductionInventoryReference.ProductionOrderId));

            return DeleteAsync(id, parameters);
        }

        public int DeleteByInventoryRequestId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(ProductionInventoryReference.InventoryRequestLineId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByInventoryRequestIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(ProductionInventoryReference.InventoryRequestLineId));

            return DeleteAsync(id, parameters);
        }
    }
}