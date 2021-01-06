using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class SupplierOrderLineReferenceService : ServiceBase<SupplierOrderLineReference>,
        ISupplierOrderLineReferenceService
    {
        public SupplierOrderLineReferenceService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<SupplierOrderLineReferenceService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
            IsKeepTracking = false;
            EnabledSoftDelete = false;
            AddEditProcedureName = "AddEditSupplierOrderLineReference";
            DeleteProcedureName = "DeleteSupplierOrderLineReference";
        }

        public int DeleteBySupplierOrderLineId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(SupplierOrderLineReference.SupplierOrderLineId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteBySupplierOrderLineAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(SupplierOrderLineReference.SupplierOrderLineId));

            return DeleteAsync(id, parameters);
        }

        public int DeleteByInventoryRequestLineId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(SupplierOrderLineReference.InventoryRequestLineId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByInventoryRequestLineIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(SupplierOrderLineReference.InventoryRequestLineId));

            return DeleteAsync(id, parameters);
        }
    }
}