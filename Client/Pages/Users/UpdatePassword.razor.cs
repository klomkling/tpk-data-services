using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Client.Services;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Pages.Users
{
    public class UpdatePasswordBase : TgMinimalComponentBase
    {
        protected readonly UpdatePasswordRequest UserInfo = new UpdatePasswordRequest();
        protected bool IsSuccess { get; set; } = true;
        
        protected override async Task OnInitializedAsync()
        {
            // Allow all user to update their password
            IsFreeAccess = true;
            
            await InitComponent(null, null);
            UserInfo.UserId = UserService.Id;
        }

        protected async Task UpdatePassword()
        {
            var result = await ApiService.UpdateAsync(UserInfo, "api/users/update-password");
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            IsSuccess = result.IsSuccess;
            if (result.IsSuccess)
            {
                ToastService.ShowSuccess("Successfully update password");
                NavigationManager.NavigateTo("/");
            }
        }

        protected void CancelRequest()
        {
            NavigationManager.NavigateTo("/");
        }
    }
}