using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class TransportationRequestLineReferenceService : ServiceBase<TransportationRequestLineReference>,
        ITransportationRequestLineReferenceService
    {
        public TransportationRequestLineReferenceService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<TransportationRequestLineReferenceService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
            IsKeepTracking = false;
            EnabledSoftDelete = false;
            AddEditProcedureName = "AddEditTransportationRequestLineReference";
            DeleteProcedureName = "DeleteTransportationRequestLineReference";
        }

        public int DeleteByTransportationRequestLineId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(TransportationRequestLineReference.TransportationRequestLineId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByTransportationRequestLineIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(TransportationRequestLineReference.TransportationRequestLineId));

            return DeleteAsync(id, parameters);
        }

        public int DeleteByInventoryRequestLineId(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(TransportationRequestLineReference.InventoryRequestLineId));

            return Delete(id, parameters);
        }

        public Task<int> DeleteByInventoryRequestLineIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("TableName", TableName);
            parameters.Add("Id", id);
            parameters.Add("ColumnName", nameof(TransportationRequestLineReference.InventoryRequestLineId));

            return DeleteAsync(id, parameters);
        }
    }
}