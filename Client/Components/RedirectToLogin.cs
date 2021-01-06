using Microsoft.AspNetCore.Components;

namespace Tpk.DataServices.Client.Components.Base
{
    public class RedirectToLogin : ComponentBase
    {
        [Inject] private NavigationManager NavigationManager { get; set; }

        protected override void OnInitialized()
        {
            NavigationManager.NavigateTo("/login");
        }
    }
}