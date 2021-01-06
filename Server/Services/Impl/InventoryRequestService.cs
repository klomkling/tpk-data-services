using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class InventoryRequestService : ServiceBase<InventoryRequest>, IInventoryRequestService
    {
        private readonly ILogger<InventoryRequestService> _logger;

        public InventoryRequestService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<InventoryRequestService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
            _logger = logger;
        }

        public bool ConfirmRequest(IEnumerable<int> collection, StringEnumeration transactionType)
        {
            var parameters = new DynamicParameters();
            parameters.Add(Requester, CurrentUsername);
            parameters.Add("TransactionType", transactionType.Value);
            parameters.Add("IdCollection", string.Join(",", collection));

            using var connection = GetOpenConnection();
            var transaction = connection.BeginTransaction();

            try
            {
                var result = connection.Execute("dbo.ConfirmInventoryRequest", parameters, transaction,
                    commandType: CommandType.StoredProcedure);

                IsError = false;
                transaction.Commit();

                return result > 0;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error get by id");
                return false;
            }
        }

        public async Task<bool> ConfirmRequestAsync(IEnumerable<int> collection, StringEnumeration transactionType)
        {
            var parameters = new DynamicParameters();
            parameters.Add(Requester, CurrentUsername);
            parameters.Add("TransactionType", transactionType.Value);
            parameters.Add("IdCollection", string.Join(",", collection));

            await using var connection = GetOpenConnection();
            var transaction = await connection.BeginTransactionAsync();

            try
            {
                var result = await connection.ExecuteAsync("dbo.ConfirmInventoryRequest", parameters, transaction,
                    commandType: CommandType.StoredProcedure);

                IsError = false;
                await transaction.CommitAsync();

                return result > 0;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error get by id");
                return false;
            }
        }
    }
}