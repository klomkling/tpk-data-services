using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using DevExpress.Blazor;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Tpk.DataServices.Shared.Classes;
using Tpk.DataServices.Shared.Data.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Tpk.DataServices.Client.Services.Impl
{
    public class ApiService : IApiService
    {
        private readonly string _apiUrl;
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;
        private readonly ISessionStorageService _sessionStorageService;

        public ApiService(IConfiguration configuration, HttpClient httpClient,
            ISessionStorageService sessionStorageService, ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
            _localStorageService = localStorageService;
            _apiUrl = configuration["ApiUrl"];
        }

        public bool IsSessionExpired { get; private set; }
        public bool IsError { get; private set; }
        public string ErrorMessage { get; private set; }
        public string BaseUrl { get; set; }

        public void SetBaseUrl(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(string targetUrl = null, string queryString = null)
        {
            var url = BuildUrl(targetUrl, queryString: queryString);
            if (IsError) return default;

            await ValidateTokenAsync();
            if (IsSessionExpired) return default;

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode == false)
                {
                    IsError = true;
                    SetErrorMessage(ExtractErrorMessage(await response.Content.ReadAsStringAsync()));
                    return default;
                }

                var result = JsonConvert.DeserializeObject<IEnumerable<T>>(await response.Content.ReadAsStringAsync());

                return result;
            }
            catch (Exception e)
            {
                IsError = true;
                SetErrorMessage(e.Message);
                Console.WriteLine(e.Message);
                return default;
            }
        }

        public async Task<LoadResult> LoadCustomData(string targetUrl, DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            await ValidateTokenAsync();
            if (IsSessionExpired) return default;

            if (targetUrl.StartsWith("/")) targetUrl = targetUrl.Substring(1);

            if (targetUrl.StartsWith("api/", StringComparison.InvariantCultureIgnoreCase) == false)
                targetUrl = $"api/{targetUrl}";

            using var response = await _httpClient.GetAsync(loadOptions.ConvertToGetRequestUri($"{_apiUrl}{targetUrl}"),
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);

                return await JsonSerializer.DeserializeAsync<LoadResult>(responseStream,
                    cancellationToken: cancellationToken);
            }

            IsError = true;
            SetErrorMessage(ExtractErrorMessage(await response.Content.ReadAsStringAsync(cancellationToken)));

            return default;
        }

        public async Task<T> GetAsync<T>(object id, string targetUrl = null)
        {
            var url = BuildUrl(targetUrl);
            if (IsError) return default;

            await ValidateTokenAsync();
            if (IsSessionExpired) return default;

            try
            {
                var response = await _httpClient.GetAsync($"{url}/{id}");
                if (response.IsSuccessStatusCode == false)
                {
                    IsError = true;
                    SetErrorMessage(ExtractErrorMessage(await response.Content.ReadAsStringAsync()));
                    return default;
                }

                var result = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());

                return result;
            }
            catch (Exception e)
            {
                IsError = true;
                SetErrorMessage(e.Message);
                Console.WriteLine(e.Message);
                return default;
            }
        }

        public async Task<T> GetAsync<T>(string targetUrl)
        {
            var url = BuildUrl(targetUrl);
            if (IsError) return default;

            await ValidateTokenAsync();
            if (IsSessionExpired) return default;

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode == false)
                {
                    IsError = true;
                    SetErrorMessage(ExtractErrorMessage(await response.Content.ReadAsStringAsync()));
                    return default;
                }

                var result = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                return result;
            }
            catch (Exception e)
            {
                IsError = true;
                SetErrorMessage(e.Message);
                Console.WriteLine(e.Message);
                return default;
            }
        }

        public async Task<bool> PostAsync(string targetUrl)
        {
            return await PostAsync<bool>(targetUrl);
        }

        public async Task<bool> PostAsync<TModel>(string targetUrl, IEnumerable<TModel> collection = null)
        {
            if (string.IsNullOrEmpty(targetUrl))
            {
                IsError = true;
                SetErrorMessage("No target url");
                return default;
            }

            await ValidateTokenAsync();
            if (IsSessionExpired) return default;

            var url = BuildUrl(targetUrl);
            if (IsError) return default;

            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(url)
                };

                if (collection != null)
                {
                    var jsonModel = JsonConvert.SerializeObject(collection);
                    request.Content = new StringContent(jsonModel, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode == false)
                {
                    IsError = true;
                    SetErrorMessage(ExtractErrorMessage(await response.Content.ReadAsStringAsync()));
                    return false;
                }

                var result = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
                return result;
            }
            catch (Exception e)
            {
                IsError = true;
                SetErrorMessage(e.Message);
                Console.WriteLine(e.Message);
                return default;
            }
        }

        public async Task<T> InsertAsync<T>(T model, string targetUrl = null)
        {
            var url = BuildUrl(targetUrl);
            if (IsError) return default;

            await ValidateTokenAsync();
            if (IsSessionExpired) return default;

            try
            {
                var jsonModel = JsonConvert.SerializeObject(model);
                var response = await _httpClient.PostAsync(url,
                    new StringContent(jsonModel, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode == false)
                {
                    IsError = true;
                    SetErrorMessage(ExtractErrorMessage(await response.Content.ReadAsStringAsync()));
                    return default;
                }

                var result = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());

                return result;
            }
            catch (Exception e)
            {
                IsError = true;
                SetErrorMessage(e.Message);
                Console.WriteLine(e.Message);
                return default;
            }
        }

        public async Task<T> UpdateAsync<T>(T model, string targetUrl = null)
        {
            var url = BuildUrl(targetUrl);
            if (IsError) return default;

            await ValidateTokenAsync();
            if (IsSessionExpired) return default;

            try
            {
                var jsonModel = JsonConvert.SerializeObject(model);
                var response = await _httpClient.PatchAsync(url,
                    new StringContent(jsonModel, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode == false)
                {
                    IsError = true;
                    SetErrorMessage(ExtractErrorMessage(await response.Content.ReadAsStringAsync()));
                    return default;
                }

                var result = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());

                return result;
            }
            catch (Exception e)
            {
                IsError = true;
                SetErrorMessage(e.Message);
                Console.WriteLine(e.Message);
                return default;
            }
        }

        public async Task<bool> DeleteAsync(object id, string targetUrl = null)
        {
            var url = BuildUrl(targetUrl);
            if (IsError) return default;

            await ValidateTokenAsync();
            if (IsSessionExpired) return default;

            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(url),
                    Content = new StringContent(id.ToString() ?? string.Empty, Encoding.UTF8, "application/json")
                };

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode) return true;

                IsError = true;
                SetErrorMessage(ExtractErrorMessage(await response.Content.ReadAsStringAsync()));
                return false;
            }
            catch (Exception e)
            {
                IsError = true;
                SetErrorMessage(e.Message);
                Console.WriteLine(e.Message);
                return default;
            }
        }

        public async Task<bool> RestoreAsync(object id, string targetUrl = null)
        {
            var url = BuildUrl(targetUrl);
            if (IsError) return default;

            await ValidateTokenAsync();
            if (IsSessionExpired) return default;

            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(url),
                    Content = new StringContent(id.ToString() ?? string.Empty, Encoding.UTF8, "application/json")
                };
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode) return true;

                IsError = true;
                SetErrorMessage(ExtractErrorMessage(await response.Content.ReadAsStringAsync()));
                return false;
            }
            catch (Exception e)
            {
                IsError = true;
                SetErrorMessage(e.Message);
                Console.WriteLine(e.Message);
                return default;
            }
        }

        public async Task<bool> BulkDeleteAsync(IEnumerable<int> collection, string targetUrl = null)
        {
            var url = BuildUrl(targetUrl, "bulk-delete");
            if (IsError) return default;

            await ValidateTokenAsync();
            if (IsSessionExpired) return default;

            try
            {
                var jsonModel = JsonConvert.SerializeObject(collection);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(url),
                    Content = new StringContent(jsonModel, Encoding.UTF8, "application/json")
                };
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode) return true;

                IsError = true;
                SetErrorMessage(ExtractErrorMessage(await response.Content.ReadAsStringAsync()));
                return false;
            }
            catch (Exception e)
            {
                IsError = true;
                SetErrorMessage(e.Message);
                Console.WriteLine(e.Message);
                return default;
            }
        }

        public async Task<bool> BulkRestoreAsync(IEnumerable<int> collection, string targetUrl = null)
        {
            var url = BuildUrl(targetUrl, "bulk-restore");
            if (IsError) return default;

            await ValidateTokenAsync();
            if (IsSessionExpired) return default;

            try
            {
                var jsonModel = JsonConvert.SerializeObject(collection);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(url),
                    Content = new StringContent(jsonModel, Encoding.UTF8, "application/json")
                };
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode) return true;

                IsError = true;
                SetErrorMessage(ExtractErrorMessage(await response.Content.ReadAsStringAsync()));
                return false;
            }
            catch (Exception e)
            {
                IsError = true;
                SetErrorMessage(e.Message);
                Console.WriteLine(e.Message);
                return default;
            }
        }

        public async Task<bool> IsUniqueValueAsync(ValidationRequest model, string targetUrl = null)
        {
            if (string.IsNullOrEmpty(targetUrl))
            {
                IsError = true;
                SetErrorMessage("Target url for unique validation does not provided");
                return false;
            }

            await ValidateTokenAsync();
            if (IsSessionExpired) return false;

            try
            {
                // Create json body
                var jsonModel = JsonConvert.SerializeObject(model);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{_apiUrl}{targetUrl}"),
                    Content = new StringContent(jsonModel, Encoding.UTF8, "application/json")
                };

                // Send request
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode == false)
                {
                    IsError = true;
                    SetErrorMessage(ExtractErrorMessage(await response.Content.ReadAsStringAsync()));
                    return default;
                }

                var result =
                    JsonConvert.DeserializeObject<ValidationResponse>(await response.Content.ReadAsStringAsync());
                return result.IsUnique;
            }
            catch (Exception e)
            {
                IsError = true;
                SetErrorMessage(e.Message);
                Console.WriteLine(e.Message);
                return default;
            }
        }

        private string BuildUrl(string targetUrl, string action = null, string queryString = null)
        {
            IsError = string.IsNullOrEmpty(BaseUrl) && string.IsNullOrEmpty(targetUrl);
            if (IsError)
            {
                SetErrorMessage("BaseUrl or targetUrl does not provided");
                return null;
            }

            if (string.IsNullOrEmpty(queryString) == false && queryString.StartsWith("?") == false)
                queryString = $"?{queryString}";

            if (string.IsNullOrEmpty(targetUrl))
                return string.IsNullOrEmpty(action)
                    ? $"{_apiUrl}{BaseUrl}{queryString}"
                    : $"{_apiUrl}{BaseUrl}/{action}{queryString}";

            if (targetUrl.StartsWith("/")) targetUrl = targetUrl.Substring(1);

            if (string.IsNullOrEmpty(action))
                return targetUrl.StartsWith("api/", StringComparison.InvariantCultureIgnoreCase)
                    ? $"{_apiUrl}{targetUrl}{queryString}"
                    : $"{_apiUrl}{BaseUrl}/{targetUrl}{queryString}";

            return targetUrl.StartsWith("api/", StringComparison.InvariantCultureIgnoreCase)
                ? $"{_apiUrl}{targetUrl}/{action}{queryString}"
                : $"{_apiUrl}{BaseUrl}/{targetUrl}/{action}{queryString}";
        }

        private async Task ValidateTokenAsync()
        {
            var content = await _sessionStorageService.GetItemAsync<string>("userState");
            if (string.IsNullOrWhiteSpace(content))
            {
                IsError = true;
                SetErrorMessage("Token is required");
                return;
            }

            var userState = content.ToUserState();

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", userState.JwtToken);
            var response = await _httpClient.PostAsync($"{_apiUrl}api/token/validate", new StringContent(string.Empty));

            if (response.IsSuccessStatusCode == false)
            {
                var url = BuildUrl("api/token/refresh-token");
                var jsonModel = JsonConvert.SerializeObject(userState);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(url),
                    Content = new StringContent(jsonModel, Encoding.UTF8, "application/json")
                };


                // Send request
                response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode == false)
                {
                    IsError = false;
                    SetErrorMessage(ExtractErrorMessage(await response.Content.ReadAsStringAsync()));
                    IsSessionExpired = true;
                    return;
                }

                userState =
                    JsonConvert.DeserializeObject<AuthenticateResponse>(await response.Content.ReadAsStringAsync());

                await _sessionStorageService.SetItemAsync("userState", await response.Content.ReadAsStringAsync());

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("bearer", userState.JwtToken);

                IsSessionExpired = false;
                return;
            }

            IsSessionExpired = false;
        }

        private string ExtractErrorMessage(string source)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<ErrorMessage>(source);
                return result.Message;
            }
            catch (Exception e)
            {
                IsError = true;
                SetErrorMessage(e.Message);
                Console.WriteLine(e.Message);
                return default;
            }
        }

        private void SetErrorMessage(string message = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                // _localStorageService.RemoveItemAsync("errorMessage");
            }
            else
            {
                _localStorageService.SetItemAsync("errorMessage", message);
            }

            ErrorMessage = message;
            Console.WriteLine($"Error: {message}");
        }
    }
}