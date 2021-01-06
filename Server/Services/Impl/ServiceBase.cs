using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pluralize.NET.Core;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public abstract class ServiceBase<T> : IServiceBase<T>
    {
        #region Constructor

        protected ServiceBase(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<ServiceBase<T>> logger)
        {
            _logger = logger;
            var appSettings = options.Value;
            _connectionString = configuration.GetConnectionString(appSettings.ConnectionName);
            TableName = PluralName(typeof(T).Name);

            CurrentUser = (User) httpContextAccessor.HttpContext?.Items["User"];
            Username = CurrentUser?.Username;

            SoftDeleteProcedureName = "dbo.SoftDeleteRecord";
            RestoreDeletedProcedureName = "dbo.RestoreDeletedRecord";
            DeleteProcedureName = "dbo.DeleteRecord";
            BulkSoftDeleteProcedureName = "dbo.BulkSoftDeleteRecords";
            BulkRestoreDeletedProcedureName = "dbo.BulkRestoreDeletedRecords";
            BulkDeleteProcedureName = "dbo.BulkDeleteRecords";
        }

        #endregion

        #region Protected functions

        protected static string ProcedureName(string prefix)
        {
            return $"dbo.{prefix}{SingularName(typeof(T).Name)}";
        }

        #endregion

        #region Fields

        private readonly ILogger<ServiceBase<T>> _logger;
        private readonly string _connectionString;

        #endregion

        #region Protected Fields

        protected string Username;
        protected string RequesterName;
        protected bool EnabledSoftDelete = true;
        protected IEnumerable<string> ExceptColumns;

        #endregion

        #region Properties

        public string TableName { get; }
        public Exception Exception { get; protected set; }
        public User CurrentUser { get; set; }
        public bool IsKeepTracking { get; protected set; } = true;
        public bool IsError { get; protected set; }
        public string PrimaryKey => "Id";
        public string AddEditProcedureName { get; protected set; }
        public string SoftDeleteProcedureName { get; protected set; }
        public string RestoreDeletedProcedureName { get; protected set; }
        public string DeleteProcedureName { get; protected set; }
        public string BulkSoftDeleteProcedureName { get; protected set; }
        public string BulkRestoreDeletedProcedureName { get; protected set; }
        public string BulkDeleteProcedureName { get; protected set; }
        public string CurrentUsername => string.IsNullOrEmpty(RequesterName) ? Username : RequesterName;
        protected string Requester => "Requester";

        #endregion

        #region Get

        public virtual TModel Get<TModel>(int id, bool isActive = true)
        {
            if (EnabledSoftDelete == false)
            {
                isActive = false;
            }

            var table = typeof(T) == typeof(TModel) ? TableName : PluralName(typeof(TModel).Name);
            var sql = SelectQueryBuilder(table, null, isActive, $"{PrimaryKey} = @{PrimaryKey}");
            var parameters = new DynamicParameters();
            parameters.Add(PrimaryKey, id);

            using var connection = GetOpenConnection();

            try
            {
                var result = connection.QueryFirstOrDefault<TModel>(sql, parameters);

                IsError = false;

                return result;
            }
            catch (Exception e)
            {
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error get by id");

                return default;
            }
        }

        public virtual async Task<TModel> GetAsync<TModel>(int id, bool isActive = true)
        {
            if (EnabledSoftDelete == false)
            {
                isActive = false;
            }

            var table = typeof(T) == typeof(TModel) ? TableName : PluralName(typeof(TModel).Name);
            var sql = SelectQueryBuilder(table, null, isActive, $"{PrimaryKey} = @{PrimaryKey}");
            var parameters = new DynamicParameters();
            parameters.Add(PrimaryKey, id);

            await using var connection = GetOpenConnection();

            try
            {
                var result = await connection.QueryFirstOrDefaultAsync<TModel>(sql, parameters);

                IsError = false;

                return result;
            }
            catch (Exception e)
            {
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error get by id async");

                return default;
            }
        }

        public TModel GetFirstOrDefault<TModel>(bool isActive = true, string orderClause = null,
            params string[] conditions)
        {
            try
            {
                var collection = GetAll<TModel>(isActive, orderClause, conditions);
                if (IsError) return default;

                var result = collection.FirstOrDefault();
                return result;
            }
            catch (Exception e)
            {
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error get first or default");

                return default;
            }
        }

        public async Task<TModel> GetFirstOrDefaultAsync<TModel>(bool isActive = true, string orderClause = null,
            params string[] conditions)
        {
            try
            {
                var collection = await GetAllAsync<TModel>(isActive, orderClause, conditions);
                if (IsError) return default;

                var result = collection.FirstOrDefault();
                return result;
            }
            catch (Exception e)
            {
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error get first or default");

                return default;
            }
        }

        public virtual IEnumerable<TModel> GetAll<TModel>(bool isActive = true, string orderClause = null,
            params string[] conditions)
        {
            if (EnabledSoftDelete == false)
            {
                isActive = false;
            }

            var table = typeof(T) == typeof(TModel) ? TableName : PluralName(typeof(TModel).Name);
            var sql = SelectQueryBuilder(table, orderClause, isActive, conditions);

            using var connection = GetOpenConnection();

            try
            {
                var result = connection.Query<TModel>(sql);

                IsError = false;

                return result;
            }
            catch (Exception e)
            {
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error get all");

                return default;
            }
        }

        public virtual async Task<IEnumerable<TModel>> GetAllAsync<TModel>(bool isActive = true,
            string orderClause = null, params string[] conditions)
        {
            if (EnabledSoftDelete == false)
            {
                isActive = false;
            }

            var table = typeof(T) == typeof(TModel) ? TableName : PluralName(typeof(TModel).Name);
            var sql = SelectQueryBuilder(table, orderClause, isActive, conditions);

            await using var connection = GetOpenConnection();

            try
            {
                var result = await connection.QueryAsync<TModel>(sql);

                IsError = false;

                return result;
            }
            catch (Exception e)
            {
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error get all async");

                return default;
            }
        }

        public virtual IEnumerable<TModel> Search<TModel>(bool isActive = true, string orderClause = null,
            params string[] conditions)
        {
            if (EnabledSoftDelete == false)
            {
                isActive = false;
            }

            var table = typeof(T) == typeof(TModel) ? TableName : PluralName(typeof(TModel).Name);
            var sql = SelectQueryBuilder(table, null, isActive, conditions);

            using var connection = GetOpenConnection();

            try
            {
                var result = connection.Query<TModel>(sql);

                IsError = false;

                return result;
            }
            catch (Exception e)
            {
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error get all active");

                return default;
            }
        }

        public async Task<IEnumerable<TModel>> SearchAsync<TModel>(bool isActive = true, string orderClause = null,
            params string[] conditions)
        {
            if (EnabledSoftDelete == false)
            {
                isActive = false;
            }

            var table = typeof(T) == typeof(TModel) ? TableName : PluralName(typeof(TModel).Name);
            var sql = SelectQueryBuilder(table, orderClause, isActive, conditions);

            await using var connection = GetOpenConnection();

            try
            {
                var result = await connection.QueryAsync<TModel>(sql);

                IsError = false;

                return result;
            }
            catch (Exception e)
            {
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error get all active");

                return default;
            }
        }

        #endregion

        #region Insert / Update

        public virtual T InsertUpdate(T model)
        {
            var sql = string.IsNullOrEmpty(AddEditProcedureName) ? ProcedureName("AddEdit") : AddEditProcedureName;
            var parameters = CompactParameters(model);
            if (IsKeepTracking)
            {
                parameters.Add(Requester, string.IsNullOrEmpty(RequesterName) ? Username : RequesterName);
            }

            using var connection = GetOpenConnection();
            var transaction = connection.BeginTransaction();

            try
            {
                var result = connection.QueryFirstOrDefault<T>(sql, parameters, transaction,
                    commandType: CommandType.StoredProcedure);

                IsError = false;
                transaction.Commit();

                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error insert or update");

                return default;
            }
        }

        public virtual async Task<T> InsertUpdateAsync(T model)
        {
            var sql = string.IsNullOrEmpty(AddEditProcedureName) ? ProcedureName("AddEdit") : AddEditProcedureName;
            var parameters = CompactParameters(model);
            if (IsKeepTracking)
            {
                parameters.Add(Requester, string.IsNullOrEmpty(RequesterName) ? Username : RequesterName);
            }

            await using var connection = GetOpenConnection();
            var transaction = await connection.BeginTransactionAsync();

            try
            {
                var result = await connection.QueryFirstOrDefaultAsync<T>(sql, parameters, transaction,
                    commandType: CommandType.StoredProcedure);

                IsError = false;
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error insert or update async");

                return default;
            }
        }

        #endregion

        #region Delete

        public virtual int BulkDelete(IEnumerable<int> idCollection)
        {
            using var connection = GetOpenConnection();
            var transaction = connection.BeginTransaction();

            try
            {
                var result = connection.Execute(BulkDeleteProcedureName, new
                {
                    TableName,
                    IdCollection = string.Join(",", idCollection),
                    IsForceDelete = EnabledSoftDelete == false
                }, transaction, commandType: CommandType.StoredProcedure);

                IsError = false;
                transaction.Commit();

                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error bulk delete");

                return -1;
            }
            finally
            {
                connection.Close();
            }
        }

        public virtual async Task<int> BulkDeleteAsync(IEnumerable<int> idCollection)
        {
            await using var connection = GetOpenConnection();
            var transaction = await connection.BeginTransactionAsync();

            try
            {
                var result = await connection.ExecuteAsync(BulkDeleteProcedureName, new
                {
                    TableName,
                    IdCollection = string.Join(",", idCollection),
                    IsForceDelete = EnabledSoftDelete == false
                }, transaction, commandType: CommandType.StoredProcedure);

                IsError = false;
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error bulk delete async");

                return -1;
            }
        }

        public virtual int BulkSoftDelete(IEnumerable<int> idCollection)
        {
            using var connection = GetOpenConnection();
            var transaction = connection.BeginTransaction();

            try
            {
                var result = connection.Execute(BulkSoftDeleteProcedureName, new
                {
                    TableName,
                    IdCollection = string.Join(",", idCollection),
                    Requester = string.IsNullOrEmpty(RequesterName) ? CurrentUser.Username : RequesterName
                }, transaction, commandType: CommandType.StoredProcedure);

                IsError = false;
                transaction.Commit();

                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error bulk soft delete");

                return -1;
            }
        }

        public virtual async Task<int> BulkSoftDeleteAsync(IEnumerable<int> idCollection)
        {
            await using var connection = GetOpenConnection();
            var transaction = await connection.BeginTransactionAsync();

            try
            {
                var result = await connection.ExecuteAsync(BulkSoftDeleteProcedureName, new
                {
                    TableName,
                    IdCollection = string.Join(",", idCollection),
                    Requester = string.IsNullOrEmpty(RequesterName) ? CurrentUser.Username : RequesterName
                }, transaction, commandType: CommandType.StoredProcedure);

                IsError = false;
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error bulk soft delete async");

                return -1;
            }
        }

        public virtual int Delete(int id, DynamicParameters parameters = null)
        {
            using var connection = GetOpenConnection();
            var transaction = connection.BeginTransaction();

            if (parameters == null)
            {
                parameters = new DynamicParameters();
                parameters.Add("TableName", TableName);
                parameters.Add("Id", id);
                parameters.Add("IsForceDelete", EnabledSoftDelete == false);
            }

            try
            {
                var result = connection.Execute(DeleteProcedureName, parameters, transaction,
                    commandType: CommandType.StoredProcedure);

                IsError = false;
                transaction.Commit();

                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error delete");

                return -1;
            }
        }

        public virtual async Task<int> DeleteAsync(int id, DynamicParameters parameters = null)
        {
            await using var connection = GetOpenConnection();
            var transaction = await connection.BeginTransactionAsync();

            if (parameters == null)
            {
                parameters = new DynamicParameters();
                parameters.Add("TableName", TableName);
                parameters.Add("Id", id);
                parameters.Add("IsForceDelete", EnabledSoftDelete == false);
            }

            try
            {
                var result = await connection.ExecuteAsync(DeleteProcedureName, parameters, transaction,
                    commandType: CommandType.StoredProcedure);

                IsError = false;
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error delete async");

                return -1;
            }
        }

        public virtual int SoftDelete(int id)
        {
            using var connection = GetOpenConnection();
            var transaction = connection.BeginTransaction();

            try
            {
                var result = connection.Execute(SoftDeleteProcedureName, new
                {
                    TableName,
                    Id = id,
                    Requester = string.IsNullOrEmpty(RequesterName) ? Username : RequesterName
                }, transaction, commandType: CommandType.StoredProcedure);

                IsError = false;
                transaction.Commit();

                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error soft delete");

                return default;
            }
        }

        public virtual async Task<int> SoftDeleteAsync(int id)
        {
            await using var connection = GetOpenConnection();
            var transaction = await connection.BeginTransactionAsync();

            try
            {
                var result = await connection.ExecuteAsync(SoftDeleteProcedureName, new
                {
                    TableName,
                    Id = id,
                    Requester = string.IsNullOrEmpty(RequesterName) ? Username : RequesterName
                }, transaction, commandType: CommandType.StoredProcedure);

                IsError = false;
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error soft delete async");

                return default;
            }
        }

        #endregion

        #region Restore

        public virtual int BulkRestore(IEnumerable<int> idCollection)
        {
            if (EnabledSoftDelete == false) return 0;

            using var connection = GetOpenConnection();
            var transaction = connection.BeginTransaction();

            try
            {
                var result = connection.Execute(BulkRestoreDeletedProcedureName, new
                {
                    TableName,
                    IdCollection = string.Join(",", idCollection)
                }, transaction, commandType: CommandType.StoredProcedure);

                IsError = false;
                transaction.Commit();

                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error bulk restore");

                return -1;
            }
        }

        public virtual async Task<int> BulkRestoreAsync(IEnumerable<int> idCollection)
        {
            if (EnabledSoftDelete == false) return 0;

            await using var connection = GetOpenConnection();
            var transaction = await connection.BeginTransactionAsync();

            try
            {
                var result = await connection.ExecuteAsync(BulkRestoreDeletedProcedureName, new
                {
                    TableName,
                    IdCollection = string.Join(",", idCollection)
                }, transaction, commandType: CommandType.StoredProcedure);

                IsError = false;
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error bulk restore async");

                return -1;
            }
        }

        public virtual int Restore(int id)
        {
            if (EnabledSoftDelete == false) return 0;

            using var connection = GetOpenConnection();
            var transaction = connection.BeginTransaction();

            try
            {
                var result = connection.Execute(RestoreDeletedProcedureName, new
                {
                    TableName,
                    Id = id
                }, transaction, commandType: CommandType.StoredProcedure);

                IsError = false;
                transaction.Commit();

                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error restore");

                return -1;
            }
        }

        public virtual async Task<int> RestoreAsync(int id)
        {
            if (EnabledSoftDelete == false) return 0;

            await using var connection = GetOpenConnection();
            var transaction = await connection.BeginTransactionAsync();

            try
            {
                var result = await connection.ExecuteAsync(RestoreDeletedProcedureName, new
                {
                    TableName,
                    Id = id
                }, transaction, commandType: CommandType.StoredProcedure);

                IsError = false;
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error restore async");

                return -1;
            }
        }

        #endregion

        #region Query / Execute

        public virtual IEnumerable<dynamic> Query(string query, object param = null,
            CommandType commandType = CommandType.Text)
        {
            using var connection = GetOpenConnection();
            var transaction = connection.BeginTransaction();

            try
            {
                var result = connection.Query(query, param, transaction, commandType: commandType);

                IsError = false;
                transaction.Commit();

                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error execute query");

                return default;
            }
        }

        public virtual async Task<IEnumerable<dynamic>> QueryAsync(string query, object param = null,
            CommandType commandType = CommandType.Text)
        {
            await using var connection = GetOpenConnection();
            var transaction = await connection.BeginTransactionAsync();

            try
            {
                var result = await connection.QueryAsync(query, param, transaction, commandType: commandType);

                IsError = false;
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error execute query async");

                return default;
            }
        }

        public virtual IEnumerable<TType> Query<TType>(string query, object param = null,
            CommandType commandType = CommandType.Text)
        {
            using var connection = GetOpenConnection();
            var transaction = connection.BeginTransaction();

            try
            {
                var result = connection.Query<TType>(query, param, transaction, commandType: commandType);

                IsError = false;
                transaction.Commit();

                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error execute query with type");

                return default;
            }
        }

        public virtual async Task<IEnumerable<TType>> QueryAsync<TType>(string query, object param = null,
            CommandType commandType = CommandType.Text)
        {
            await using var connection = GetOpenConnection();
            var transaction = await connection.BeginTransactionAsync();

            try
            {
                var result = await connection.QueryAsync<TType>(query, param, transaction, commandType: commandType);

                IsError = false;
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error execute query with type async");

                return default;
            }
        }

        public virtual dynamic QueryFirstOrDefault(string query, object param = null,
            CommandType commandType = CommandType.Text)
        {
            using var connection = GetOpenConnection();
            var transaction = connection.BeginTransaction();

            try
            {
                var result = connection.QueryFirstOrDefault(query, param, transaction, commandType: commandType);

                IsError = false;
                transaction.Commit();

                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error execute query first or default");

                return default;
            }
        }

        public virtual async Task<dynamic> QueryFirstOrDefaultAsync(string query, object param = null,
            CommandType commandType = CommandType.Text)
        {
            await using var connection = GetOpenConnection();
            var transaction = await connection.BeginTransactionAsync();

            try
            {
                var result =
                    await connection.QueryFirstOrDefaultAsync(query, param, transaction, commandType: commandType);

                IsError = false;
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error execute query first or default async");

                return default;
            }
        }

        public virtual TType QueryFirstOrDefault<TType>(string query, object param = null,
            CommandType commandType = CommandType.Text)
        {
            using var connection = GetOpenConnection();
            var transaction = connection.BeginTransaction();

            try
            {
                var result = connection.QueryFirstOrDefault<TType>(query, param, transaction, commandType: commandType);

                IsError = false;
                transaction.Commit();

                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error execute query first or default with type");

                return default;
            }
        }

        public virtual async Task<TType> QueryFirstOrDefaultAsync<TType>(string query, object param = null,
            CommandType commandType = CommandType.Text)
        {
            await using var connection = GetOpenConnection();
            var transaction = await connection.BeginTransactionAsync();

            try
            {
                var result =
                    await connection.QueryFirstOrDefaultAsync<TType>(query, param, transaction,
                        commandType: commandType);

                IsError = false;
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error execute query first or default with type async");

                return default;
            }
        }

        public int Execute(string query, object param = null, CommandType commandType = CommandType.Text)
        {
            using var connection = GetOpenConnection();
            var transaction = connection.BeginTransaction();

            try
            {
                var result = connection.Execute(query, param, transaction, commandType: commandType);

                IsError = false;
                transaction.Commit();

                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error execute");

                return -1;
            }
        }

        public async Task<int> ExecuteAsync(string query, object param = null,
            CommandType commandType = CommandType.Text)
        {
            await using var connection = GetOpenConnection();
            var transaction = await connection.BeginTransactionAsync();

            try
            {
                var result = await connection.ExecuteAsync(query, param, transaction, commandType: commandType);

                IsError = false;
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error execute async");

                return -1;
            }
        }

        public bool IsExists<TModel>(bool isActive = false, params string[] conditions)
        {
            var table = typeof(T) == typeof(TModel) ? TableName : PluralName(typeof(TModel).Name);
            var sql = SelectQueryBuilder(table, null, isActive, conditions).Replace("*", "COUNT (1)");

            using var connection = GetOpenConnection();

            try
            {
                var result = connection.QueryFirstOrDefault<int>(sql);

                IsError = false;

                return result > 0;
            }
            catch (Exception e)
            {
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error get all");

                return default;
            }
        }

        public async Task<bool> IsExistsAsync<TModel>(bool isActive = false, params string[] conditions)
        {
            var table = typeof(T) == typeof(TModel) ? TableName : PluralName(typeof(TModel).Name);
            var sql = SelectQueryBuilder(table, null, isActive, conditions).Replace("*", "COUNT (1)");

            await using var connection = GetOpenConnection();

            try
            {
                var result = await connection.QueryFirstOrDefaultAsync<int>(sql);

                IsError = false;

                return result > 0;
            }
            catch (Exception e)
            {
                IsError = true;
                Exception = e;
                _logger.LogError(e, "Error get all");

                return default;
            }
        }

        #endregion

        #region SqlBuilder

        protected string SelectQueryBuilder(string tableName, string orderClause, bool isActive = false,
            params string[] conditions)
        {
            var isWhereExists = false;
            var sb = new StringBuilder();
            sb.AppendLine("SELECT *");
            sb.AppendLine($"FROM dbo.{tableName}");

            if (isActive)
            {
                isWhereExists = true;
                sb.AppendLine("WHERE DeletedAt IS NULL");
                sb.AppendLine("AND DeletedBy IS NULL");
            }

            foreach (var condition in conditions)
            {
                if (string.IsNullOrEmpty(condition)) continue;

                sb.AppendLine($"{(isWhereExists ? "AND" : "WHERE")} {condition}");
                isWhereExists = true;
            }

            if (string.IsNullOrEmpty(orderClause) == false) sb.AppendLine($"ORDER BY {orderClause}");

            return sb.ToString();
        }

        private string UpdateQueryBuilder(string whereClause, params string[] columns)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"UPDATE dbo.{TableName} SET");

            for (var index = 0; index < columns.Length; index++)
                sb.AppendLine($"{columns[index]}{(index < columns.Length - 1 ? "," : string.Empty)}");

            sb.AppendLine($"WHERE {whereClause}");

            return sb.ToString();
        }

        private string DeleteQueryBuilder(string whereClause)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"DELETE FROM dbo.{TableName}");
            sb.AppendLine($"WHERE {whereClause}");
            sb.AppendLine("AND DeletedAt IS NOT NULL");
            sb.AppendLine("AND DeletedBy IS NOT NULL");

            return sb.ToString();
        }

        #endregion

        #region Misc

        public DbConnection GetOpenConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();

            return connection;
        }

        public virtual DynamicParameters CompactParameters(T model)
        {
            var exceptionLists = new List<string>
            {
                "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "DeletedStatus"
            };
            if (ExceptColumns != null) exceptionLists.AddRange(ExceptColumns);

            var parameters = new DynamicParameters();
            foreach (var prop in model.GetType().GetProperties())
            {
                if (exceptionLists.Contains(prop.Name)) continue;

                // Check if property have NotMapped Attribute, then not include into parameters
                if (prop.GetCustomAttributes(true).FirstOrDefault(a => a.GetType() == typeof(NotMappedAttribute)) !=
                    null)
                    continue;

                parameters.Add(prop.Name, prop.GetValue(model, default));
            }

            return parameters;
        }

        public void SetRequester(string username)
        {
            RequesterName = username;
        }

        protected static string PluralName(string name)
        {
            var converter = new Pluralizer();
            return converter.Pluralize(name);
        }

        protected static string SingularName(string name)
        {
            var converter = new Pluralizer();
            return converter.Singularize(name);
        }

        #endregion
    }
}