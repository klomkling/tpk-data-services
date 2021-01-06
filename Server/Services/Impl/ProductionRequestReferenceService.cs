using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class ProductionRequestReferenceService : ServiceBase<ProductionRequestReference>,
        IProductionRequestReferenceService
    {
        public ProductionRequestReferenceService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<ProductionRequestReferenceService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
            IsKeepTracking = false;
            EnabledSoftDelete = false;
            AddEditProcedureName = "AddEditProductionRequestReference";
            DeleteProcedureName = "DeleteProductionRequestReference";
        }

        public int DeleteByCustomerOrderId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(ProductionRequestReference.CustomerOrderId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByCustomerOrderIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(ProductionRequestReference.CustomerOrderId));

            return DeleteAsync(id, parameters);
        }

        public int DeleteByProductionRequestId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(ProductionRequestReference.ProductionRequestId));

            return Delete(id, parameters);

        }

        public Task<int> DeleteByProductionRequestIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(ProductionRequestReference.ProductionRequestId));

            return DeleteAsync(id, parameters);
        }
    }
}