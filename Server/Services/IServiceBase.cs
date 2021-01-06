using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services
{
    public interface IServiceBase<T>
    {
        #region Properties

        string TableName { get; }
        string PrimaryKey { get; }
        string AddEditProcedureName { get; }
        string SoftDeleteProcedureName { get; }
        string RestoreDeletedProcedureName { get; }
        string DeleteProcedureName { get; }
        string BulkSoftDeleteProcedureName { get; }
        string BulkRestoreDeletedProcedureName { get; }
        string BulkDeleteProcedureName { get; }
        User CurrentUser { get; set; }
        string CurrentUsername { get; }
        bool IsKeepTracking { get; }
        bool IsError { get; }
        Exception Exception { get; }

        #endregion

        #region Get

        TModel Get<TModel>(int id, bool isActive = true);
        Task<TModel> GetAsync<TModel>(int id, bool isActive = true);

        TModel GetFirstOrDefault<TModel>(bool isActive = true, string orderClause = null, params string[] conditions);

        Task<TModel> GetFirstOrDefaultAsync<TModel>(bool isActive = true, string orderClause = null,
            params string[] conditions);

        IEnumerable<TModel> GetAll<TModel>(bool isActive = true, string orderClause = null, params string[] conditions);

        Task<IEnumerable<TModel>> GetAllAsync<TModel>(bool isActive = true, string orderClause = null,
            params string[] conditions);

        IEnumerable<TModel> Search<TModel>(bool isActive = true, string orderClause = null, params string[] conditions);

        Task<IEnumerable<TModel>> SearchAsync<TModel>(bool isActive = true, string orderClause = null,
            params string[] conditions);

        #endregion

        #region Insert/Update

        T InsertUpdate(T model);
        Task<T> InsertUpdateAsync(T model);

        #endregion

        #region Delete

        int BulkDelete(IEnumerable<int> idCollection);
        Task<int> BulkDeleteAsync(IEnumerable<int> idCollection);
        int BulkSoftDelete(IEnumerable<int> idCollection);
        Task<int> BulkSoftDeleteAsync(IEnumerable<int> idCollection);
        int Delete(int id, DynamicParameters parameters = null);
        Task<int> DeleteAsync(int id, DynamicParameters parameters = null);
        int SoftDelete(int id);
        Task<int> SoftDeleteAsync(int id);

        #endregion

        #region Restore

        int BulkRestore(IEnumerable<int> idCollection);
        Task<int> BulkRestoreAsync(IEnumerable<int> idCollection);
        int Restore(int id);
        Task<int> RestoreAsync(int id);

        #endregion

        #region Query / Execute

        IEnumerable<dynamic> Query(string query, object param = null, CommandType commandType = CommandType.Text);

        Task<IEnumerable<dynamic>> QueryAsync(string query, object param = null,
            CommandType commandType = CommandType.Text);

        IEnumerable<TType> Query<TType>(string query, object param = null, CommandType commandType = CommandType.Text);

        Task<IEnumerable<TType>> QueryAsync<TType>(string query, object param = null,
            CommandType commandType = CommandType.Text);

        dynamic QueryFirstOrDefault(string query, object param = null, CommandType commandType = CommandType.Text);

        Task<dynamic> QueryFirstOrDefaultAsync(string query, object param = null,
            CommandType commandType = CommandType.Text);

        TType QueryFirstOrDefault<TType>(string query, object param = null, CommandType commandType = CommandType.Text);

        Task<TType> QueryFirstOrDefaultAsync<TType>(string query, object param = null,
            CommandType commandType = CommandType.Text);

        int Execute(string query, object param = null, CommandType commandType = CommandType.Text);
        Task<int> ExecuteAsync(string query, object param = null, CommandType commandType = CommandType.Text);

        #endregion

        #region Misc

        bool IsExists<TModel>(bool isActive = false, params string[] conditions);
        Task<bool> IsExistsAsync<TModel>(bool isActive = false, params string[] conditions);
        DbConnection GetOpenConnection();
        void SetRequester(string username);
        DynamicParameters CompactParameters(T model);

        #endregion
    }
}