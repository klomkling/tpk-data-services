using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class CustomerOrderReferenceService : ServiceBase<CustomerOrderReference>, ICustomerOrderReferenceService
    {
        public CustomerOrderReferenceService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<CustomerOrderReferenceService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
            IsKeepTracking = false;
            EnabledSoftDelete = false;
            AddEditProcedureName = "AddEditCustomerOrderReference";
            DeleteProcedureName = "DeleteCustomerOrderReference";
        }

        public int DeleteByCustomerOrderId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(CustomerOrderReference.CustomerOrderId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByCustomerOrderIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(CustomerOrderReference.CustomerOrderId));

            return DeleteAsync(id, parameters);
        }

        public int DeleteByInventoryRequestId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(CustomerOrderReference.InventoryRequestId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByInventoryRequestIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(CustomerOrderReference.InventoryRequestId));

            return DeleteAsync(id, parameters);
        }
    }
}