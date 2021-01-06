using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class CustomerOrderLineReferenceService : ServiceBase<CustomerOrderLineReference>,
        ICustomerOrderLineReferenceService
    {
        public CustomerOrderLineReferenceService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<CustomerOrderLineReferenceService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
            IsKeepTracking = false;
            EnabledSoftDelete = false;
            AddEditProcedureName = "AddEditCustomerOrderLineReference";
            DeleteProcedureName = "DeleteCustomerOrderLineReference";
        }

        public int DeleteByCustomerOrderLineId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(CustomerOrderLineReference.CustomerOrderLineId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByCustomerOrderLineIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(CustomerOrderLineReference.CustomerOrderLineId));

            return DeleteAsync(id, parameters);
        }

        public int DeleteByInventoryRequestLineId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(CustomerOrderLineReference.InventoryRequestLineId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByInventoryRequestLineIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(CustomerOrderLineReference.InventoryRequestLineId));

            return DeleteAsync(id, parameters);
        }
    }
}