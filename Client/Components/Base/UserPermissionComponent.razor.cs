using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Components.Base
{
    public class
        UserPermissionComponentBase : TgComponentBase<UserPermission, UserPermission, UserPermissionEditContext>
    {
        protected IEnumerable<TgClaimTypes> ClaimTypeCollection;
        protected IEnumerable<TgPermissions> PermissionCollection;
        protected UserPermission UserPermission { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.Users, "api/users/permissions");

            ClaimTypeCollection = Enumeration.GetAll<TgClaimTypes>().OrderBy(e => e.DisplayName);
            PermissionCollection = Enumeration.GetAll<TgPermissions>().OrderBy(e => e.Value);
        }

        protected async Task<LoadResult> LoadUserPermissions(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            var url = $"api/users/{MasterId}/permissions";
            return await LoadDataAsync(url, loadOptions, cancellationToken);
        }

        protected override void OnBeforeRowEditing(UserPermission model = default)
        {
            DataEditContext.UserId = MasterId;
        }
    }
}