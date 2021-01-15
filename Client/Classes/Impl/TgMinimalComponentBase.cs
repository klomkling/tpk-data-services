using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Tpk.DataServices.Client.Services;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Classes.Impl
{
    public class TgMinimalComponentBase : ComponentBase
    {
        #region Inject section

        [Inject] protected IApiService ApiService { get; set; }
        [Inject] protected IUserService UserService { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected IToastService ToastService { get; set; }
        [Inject] protected ILocalStorageService LocalStorageService { get; set; }

        #endregion
        
        #region Parameters

        [Parameter] public bool ReadOnly { get; set; }
        
        #endregion
        
        #region Protected properties

        protected TgClaimTypes RequiredClaimType { get; set; }
        protected bool IsAdmin { get; set; }
        protected bool CanCreate { get; set; }
        protected bool CanEdit { get; set; }
        protected bool CanDelete { get; set; }
        protected bool CanRestore { get; set; }
        protected string ApiUrl { get; set; }
        protected bool IsValidating { get; set; } = true;
        protected bool IsFreeAccess { get; set; }
        protected bool IsOnlyAdmin { get; set; }

        #endregion

        #region Protected methods

        protected virtual async Task InitComponent(TgClaimTypes requireClaimType, string apiUrl)
        {
            ApiUrl = apiUrl;
            RequiredClaimType = requireClaimType;
            await ValidatePermission();

            ApiService.SetBaseUrl(ApiUrl);
        }

        protected void RedirectToLoginPage()
        {
            var returnUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            var url = "/login";
            if (string.IsNullOrEmpty(returnUrl) == false) url += $"?returnUrl={returnUrl}";

            NavigationManager.NavigateTo(url);
        }

        protected void RedirectToErrorPage(string errorMessage)
        {
            NavigationManager.NavigateTo("/error");
        }

        protected void RedirectToAccessDeniedPage()
        {
            NavigationManager.NavigateTo("/access-denied");
        }

        // Must validate permission on initialized
        protected async Task ValidatePermission()
        {
            await UserService.LoadAsync();
            IsAdmin = UserService.IsAdminLevel;
            var permission = await UserService.GetPermissionAsync(RequiredClaimType);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            if (IsOnlyAdmin && IsAdmin == false) RedirectToAccessDeniedPage();
            
            if (IsFreeAccess == false)
            {
                CanEdit = ((permission?.Value ?? -1) >= TgPermissions.Update.Value || IsAdmin) && ReadOnly == false;
                if ((permission?.Value ?? -1) < TgPermissions.View.Value && IsAdmin == false)
                    RedirectToAccessDeniedPage();
            }

            IsValidating = false;
        }

        #endregion
    }
}