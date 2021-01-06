using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Services
{
    public interface IApiService
    {
        bool IsSessionExpired { get; }
        bool IsError { get; }
        string ErrorMessage { get; }
        string BaseUrl { get; set; }
        void SetBaseUrl(string baseUrl);
        Task<IEnumerable<T>> GetAllAsync<T>(string targetUrl = null, string queryString = null);

        Task<LoadResult> LoadCustomData(string targetUrl, DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken);

        Task<T> GetAsync<T>(object id, string targetUrl = null);
        Task<T> GetAsync<T>(string targetUrl);
        Task<bool> PostAsync(string targetUrl);
        Task<bool> PostAsync<TModel>(string targetUrl, IEnumerable<TModel> collection = null);
        Task<T> InsertAsync<T>(T model, string targetUrl = null);
        Task<T> UpdateAsync<T>(T model, string targetUrl = null);
        Task<bool> DeleteAsync(object id, string targetUrl = null);
        Task<bool> RestoreAsync(object id, string targetUrl = null);
        Task<bool> BulkDeleteAsync(IEnumerable<int> collection, string targetUrl = null);
        Task<bool> BulkRestoreAsync(IEnumerable<int> collection, string targetUrl = null);
        Task<bool> IsUniqueValueAsync(ValidationRequest model, string targetUrl = null);
    }
}