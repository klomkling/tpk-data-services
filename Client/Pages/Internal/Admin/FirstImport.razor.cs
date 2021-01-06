using System.Threading.Tasks;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Pages.Internal.Admin
{
    public class FirstImportBase : TgMinimalComponentBase
    {
        protected bool IsClicked { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.Users, "api/admin");
        }

        protected async Task StartProcess()
        {
            var url = $"{ApiUrl}/first-import";
            var done = await ApiService.PostAsync(url);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            IsClicked = false;

            if (done == false) return;

            ToastService.ShowSuccess("Successfully generated stock request");
        }
    }
}