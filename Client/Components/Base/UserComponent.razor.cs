using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.AspNetCore.Components;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Classes;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Enums;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Components.Base
{
    public class UserComponentBase : TgComponentBase<User, User, UserEditContext>
    {
        private const string PasswordWarningText = "Leave blank if you don't want to change password";
        protected string PasswordWarning;
        protected IEnumerable<UserRole> RoleCollection;

        protected User User { get; set; }

        [Parameter] public EventCallback<User> OnSelectedUserChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.Users, "api/users");

            RoleCollection = Enumeration.GetAll<TgRoles>().ToUserRole();
        }

        protected async Task<LoadResult> LoadUsers(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            return await LoadDataAsync(ApiUrl, loadOptions, cancellationToken);
        }

        protected override void OnBeforeRowEditing(User model = default)
        {
            PasswordWarning = ComponentMode == ComponentMode.Add ? null : PasswordWarningText;
        }

        protected override Task PrepareRowForUpdate()
        {
            if (string.IsNullOrEmpty(DataEditContext.PasswordHash) == false && DataEditContext.Id > 0)
                DataEditContext.PasswordHash = $"NEW:{DataEditContext.PasswordHash}";
            DataEditContext.UpdateDataItem();

            return Task.CompletedTask;
        }

        protected override void OnSelectedCollectionChanged(IEnumerable<User> collection)
        {
            var user = SelectedCollection.FirstOrDefault(u => u.Id == SelectedId);

            OnSelectedUserChanged.InvokeAsync(user);
        }
    }
}