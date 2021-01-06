using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Tpk.DataServices.Shared.Classes;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Services.Impl
{
    public class AuthService : IAuthService
    {
        private readonly string _apiUrl;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly HttpClient _httpClient;
        private readonly ISessionStorageService _sessionStorageService;

        public AuthService(IConfiguration configuration, HttpClient httpClient,
            AuthenticationStateProvider authenticationStateProvider, ISessionStorageService sessionStorageService)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _sessionStorageService = sessionStorageService;

            _apiUrl = configuration["ApiUrl"];
        }

        public async Task<AuthenticateResponse> Login(AuthenticateRequest model)
        {
            try
            {
                // var jsonModel = JsonSerializer.Serialize(model);
                var jsonModel = JsonConvert.SerializeObject(model);
                var response = await _httpClient.PostAsync($"{_apiUrl}api/token/login",
                    new StringContent(jsonModel, Encoding.UTF8, "application/json"));
                var content = await response.Content.ReadAsStringAsync();
                var userState = content.ToUserState();

                if (response.IsSuccessStatusCode == false) return userState;

                await _sessionStorageService.SetItemAsync("userState", content);

                ((ApiAuthenticationStateProvider) _authenticationStateProvider).MarkUserAsAuthenticated(model.Username);
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("bearer", userState.JwtToken);

                return userState;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<bool> Logout()
        {
            try
            {
                var content = await _sessionStorageService.GetItemAsync<string>("userState");
                var userState = content.ToUserState();
                var token = userState.JwtToken;
                if (string.IsNullOrEmpty(token)) return true;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

                var response = await _httpClient.PostAsync($"{_apiUrl}api/token/logout",
                    new StringContent(string.Empty));

                JsonConvert.DeserializeObject<AuthenticateResponse>(await response.Content.ReadAsStringAsync());

                if (response.IsSuccessStatusCode == false) return false;

                await _sessionStorageService.RemoveItemAsync("userState");
                ((ApiAuthenticationStateProvider) _authenticationStateProvider).MarkUserAsLoggedOut();

                _httpClient.DefaultRequestHeaders.Authorization = null;

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<bool> RefreshToken()
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_apiUrl}api/token/refresh-token",
                    new StringContent(string.Empty));
                var content = await response.Content.ReadAsStringAsync();
                var userState = content.ToUserState();

                if (response.IsSuccessStatusCode == false) return false;

                await _sessionStorageService.SetItemAsync("userState", content);
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("bearer", userState.JwtToken);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}