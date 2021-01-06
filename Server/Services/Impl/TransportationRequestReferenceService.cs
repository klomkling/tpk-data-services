using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class TransportationRequestReferenceService : ServiceBase<TransportationRequestReference>,
        ITransportationRequestReferenceService
    {
        public TransportationRequestReferenceService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<TransportationRequestReferenceService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
            IsKeepTracking = false;
            EnabledSoftDelete = false;
            AddEditProcedureName = "AddEditTransportationRequestReference";
            DeleteProcedureName = "DeleteTransportationRequestReference";
        }

        public int DeleteByTransportationRequestId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(TransportationRequestReference.TransportationRequestId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByTransportationRequestIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(TransportationRequestReference.TransportationRequestId));

            return DeleteAsync(id, parameters);

        }

        public int DeleteByInventoryRequestId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(TransportationRequestReference.InventoryRequestId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByInventoryRequestIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(TransportationRequestReference.InventoryRequestId));

            return DeleteAsync(id, parameters);
        }
    }
}