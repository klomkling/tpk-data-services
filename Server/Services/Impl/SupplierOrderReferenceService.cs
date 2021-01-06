using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class SupplierOrderReferenceService : ServiceBase<SupplierOrderReference>, ISupplierOrderReferenceService
    {
        public SupplierOrderReferenceService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<SupplierOrderReferenceService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
            IsKeepTracking = false;
            EnabledSoftDelete = false;
            AddEditProcedureName = "AddEditSupplierOrderReference";
            DeleteProcedureName = "DeleteSupplierOrderReference";
        }


        public int DeleteBySupplierOrderId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(SupplierOrderReference.SupplierOrderId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteBySupplierOrderIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(SupplierOrderReference.SupplierOrderId));

            return DeleteAsync(id, parameters);
        }

        public int DeleteByInventoryRequestId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(SupplierOrderReference.InventoryRequestId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByInventoryRequestIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(SupplierOrderReference.InventoryRequestId));

            return DeleteAsync(id, parameters);
        }
    }
}