using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;
using Tpk.DataServices.Shared.Data.Views;

namespace Tpk.DataServices.Client.Components.Base
{
    public class CustomerContactComponentBase
        : TgComponentBase<CustomerContactWithDetail, CustomerContact, CustomerContactEditContext>
    {
        private ContactType _selectedContactType;

        protected CustomerContactWithDetail CustomerContactWithDetail { get; set; }

        protected ContactType SelectedContactType
        {
            get => _selectedContactType;
            set
            {
                if (Equals(_selectedContactType, value)) return;
                _selectedContactType = value;
                if (value != null) DataEditContext.ContactTypeId = value.Id;

                StateHasChanged();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.CustomerContacts, "api/customer-contacts");
        }

        protected override void OnBeforeRowEditing(CustomerContactWithDetail model = default)
        {
            DataEditContext.CustomerId = MasterId;

            SelectedContactType = null;

            if (DataEditContext.Id == 0) return;

            Task.Run(async () =>
            {
                SelectedContactType =
                    await ApiService.GetAsync<ContactType>(DataEditContext.DataItem.ContactTypeId, "api/contact-types");
            });
        }

        protected async Task<LoadResult> LoadCustomerContacts(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            var url = $"api/customers/{MasterId}/contacts";

            return await LoadDataAsync(url, loadOptions, cancellationToken);
        }

        protected async Task<IEnumerable<ContactType>> SearchContactTypes(string searchText)
        {
            var response =
                await ApiService.GetAllAsync<ContactType>(
                    $"api/contact-types/search?columns=name&searchStrings={searchText}");
            return response;
        }
    }
}