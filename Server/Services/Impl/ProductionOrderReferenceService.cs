using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class ProductionOrderReferenceService : ServiceBase<ProductionOrderReference>,
        IProductionOrderReferenceService
    {
        public ProductionOrderReferenceService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<ProductionOrderReferenceService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
            IsKeepTracking = false;
            EnabledSoftDelete = false;
            AddEditProcedureName = "AddEditProductionOrderReference";
            DeleteProcedureName = "DeleteProductionOrderReference";
        }

        public int DeleteByProductionOrderId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(ProductionOrderReference.ProductionOrderId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByProductionOrderIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(ProductionOrderReference.ProductionOrderId));

            return DeleteAsync(id, parameters);
        }

        public int DeleteByProductionRequestId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(ProductionOrderReference.ProductionRequestId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByProductionRequestIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(ProductionOrderReference.ProductionRequestId));

            return DeleteAsync(id, parameters);
        }
    }
}