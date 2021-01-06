using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Tpk.DataServices.Client.Services;
using Tpk.DataServices.Shared.Classes;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Pages
{
    public class LoginBase : ComponentBase
    {
        protected readonly AuthenticateRequest _userInfo = new AuthenticateRequest();
        protected bool _isLoginSuccess = true;
        protected string _returnUrl;
        [Inject] private IAuthService AuthService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        protected override void OnInitialized()
        {
            NavigationManager.TryGetQueryString<string>("returnUrl", out var returnUrl);
            _returnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;

            base.OnInitialized();
        }

        protected async Task LoginUser()
        {
            var result = await AuthService.Login(_userInfo);
            if (string.IsNullOrEmpty(result.Username) == false)
                NavigationManager.NavigateTo(_returnUrl, true);
            else
                _isLoginSuccess = false;
        }
    }
}