using System.Threading.Tasks;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Pages.Internal.Admin
{
    public class UsersBase : TgMinimalComponentBase
    {
        protected int UserId;
        protected bool IsBusy;
        protected bool ClientBusy;
        protected bool IsUserIsAdminLevel;

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.Users, null);
        }

        protected void SelectedUserChanged(User user)
        {
            IsUserIsAdminLevel = user.RoleId == TgRoles.Administrator.Value || user.RoleId == TgRoles.Director.Value;
            StateHasChanged();
        }
    }
}