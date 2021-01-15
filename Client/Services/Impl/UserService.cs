using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.SessionStorage;
using Tpk.DataServices.Shared.Classes;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Services.Impl
{
    public class UserService : IUserService
    {
        private readonly IApiService _apiService;
        private readonly ISessionStorageService _sessionStorageService;
        private AuthenticateResponse _userState;

        public UserService(IApiService apiService, ISessionStorageService sessionStorageService)
        {
            _apiService = apiService;
            _sessionStorageService = sessionStorageService;
        }

        public bool IsError { get; private set; }
        public string ErrorMessage { get; private set; }
        public int Id => _userState?.Id ?? 0;
        public string Username => _userState?.Username;
        public TgRoles Role { get; private set; }
        public IEnumerable<TgRoles> Roles { get; private set; }
        public bool IsAdministrator => Role.Value == TgRoles.Administrator.Value;
        public bool IsAdminLevel => Role.Value.IsAcceptedRoles(TgRoles.Administrator, TgRoles.Director);

        public async Task<AuthenticateResponse> LoadAsync()
        {
            try
            {
                Roles = Enumeration.GetAll<TgRoles>();
                var content = await _sessionStorageService.GetItemAsync<string>("userState");
                _userState = content.ToUserState();
                
                var roleId = _userState?.RoleId ?? 0;
                Role = Roles.FirstOrDefault(r => r.Value.Equals(roleId)) ?? TgRoles.Anonymous;
                
                return _userState;
            }
            catch (Exception e)
            {
                IsError = true;
                SetErrorMessage(e.Message);
                return default;
            }
        }

        public async Task<IEnumerable<UserClaim>> GetClaimsAsync(int userId = 0)
        {
            try
            {
                var id = userId == 0 ? _userState?.Id ?? 0 : userId;
                if (id == 0)
                {
                    IsError = true;
                    SetErrorMessage("UserId == 0");
                    return default;
                }

                var collection = await _apiService.GetAllAsync<UserClaim>($"api/users/{id}/claims");

                return collection;
            }
            catch (Exception e)
            {
                IsError = true;
                SetErrorMessage(e.Message);
                return default;
            }
        }

        public async Task<bool> HasPermissionAsync(TgClaimTypes claimTypes, TgPermissions tpkPermissions)
        {
            try
            {
                var claims = await GetClaimsAsync();
                var claim = claims.FirstOrDefault(c => c.ClaimId == claimTypes.Value);
                if (claim == null) return false;

                return tpkPermissions.Value >= claim.Permission;
            }
            catch (Exception e)
            {
                IsError = true;
                SetErrorMessage(e.Message);
                return default;
            }
        }

        public async Task<bool> HasPermissionOnAsync(TgClaimTypes claimTypes)
        {
            try
            {
                var claims = await GetClaimsAsync();
                var claim = claims.FirstOrDefault(c => c.ClaimId == claimTypes.Value);

                return claim?.Permission > 0;
            }
            catch (Exception e)
            {
                IsError = true;
                SetErrorMessage(e.Message);
                return default;
            }
        }

        public async Task<TgPermissions> GetPermissionAsync(TgClaimTypes claimTypes)
        {
            try
            {
                var claims = await GetClaimsAsync();
                var claim = claims.FirstOrDefault(c => c.ClaimId == claimTypes.Value);

                return Enumeration.FromValue<TgPermissions>(claim?.Permission ?? 0);
            }
            catch (Exception e)
            {
                IsError = true;
                SetErrorMessage(e.Message);
                return default;
            }
        }

        public async Task<User> GetUserAsync(int userId)
        {
            try
            {
                var user = await _apiService.GetAsync<User>(userId, "api/users");
                IsError = _apiService.IsError;
                ErrorMessage = _apiService.ErrorMessage;

                return user;
            }
            catch (Exception e)
            {
                IsError = true;
                SetErrorMessage(e.Message);
                return default;
            }
        }

        private void SetErrorMessage(string message = null)
        {
            if (string.IsNullOrEmpty(message))
                _sessionStorageService.RemoveItemAsync("errorMessage");
            else
                _sessionStorageService.SetItemAsync("errorMessage", message);
            ErrorMessage = message;
            Console.WriteLine(message);
        }
    }
}