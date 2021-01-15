using System;
using System.Threading.Tasks;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Pages.Internal.Admin
{
    public class FirstImportBase : TgMinimalComponentBase
    {
        protected bool ButtonVisible { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.Users, "api/admin");
        }

        protected async Task StartProcess()
        {
            ButtonVisible = false;
            StateHasChanged();
            
            var url = $"{ApiUrl}/first-import";
            var done = await ApiService.PostAsync(url);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            if (done == false)
            {
                Console.WriteLine("Some fail");
                return;
            }

            ToastService.ShowSuccess("Successfully generated stock request");
            NavigationManager.NavigateTo("/");
        }
    }
}