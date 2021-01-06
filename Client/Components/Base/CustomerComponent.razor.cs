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
    public class CustomerComponentBase : TgComponentBase<CustomerWithDetail, Customer, CustomerEditContext>
    {
        private Customer _selectedOemCustomer;

        protected Customer SelectedOemCustomer
        {
            get => _selectedOemCustomer;
            set
            {
                if (Equals(_selectedOemCustomer, value)) return;
                _selectedOemCustomer = value;
                DataEditContext.OemByCustomerId = value?.OemByCustomerId;
                StateHasChanged();
            }
        }
        protected CustomerWithDetail CustomerWithDetail { get; set; }
        protected readonly IEnumerable<string> YesNoDropdown = new List<string>
            {"Yes", "No"};

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.Customers, "api/customers");
        }

        protected override void OnBeforeRowEditing(CustomerWithDetail model = default)
        {
            SelectedOemCustomer = null;
            if (DataEditContext.Id == 0) return;
            if ((DataEditContext.DataItem?.OemByCustomerId ?? 0) == 0) return;

            Task.Run(async () =>
            {
                var result = await ApiService.GetAsync<Customer>(DataEditContext.DataItem.OemByCustomerId, "api/customers");
                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

                SelectedOemCustomer = result;
            });
        }

        protected async Task<LoadResult> LoadCustomers(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            return await LoadDataAsync(ApiUrl, loadOptions, cancellationToken);
        }

        protected async Task<IEnumerable<Customer>> SearchCustomers(string searchText)
        {
            var url = $"api/customers/search?columns=OemByCustomerId&searchStrings=null&columns=code&searchStrings={searchText}";
            var result = await ApiService.GetAllAsync<Customer>(url);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            return result;
        }
    }
}