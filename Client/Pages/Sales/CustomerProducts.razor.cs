using System.Collections.Generic;
using System.Threading.Tasks;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Pages.Sales
{
    public class CustomerProductsBase : TgMinimalComponentBase
    {
        private Customer _selectedCustomer;

        protected Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (Equals(_selectedCustomer, value)) return;
                _selectedCustomer = value;
                if (Equals(value, null))
                {
                    CustomerProductId = 0;
                }
                StateHasChanged();
            }
        }

        protected int CustomerProductId { get; set; } = 0;
        protected int CustomerProductSelectedCount { get; set; } = 0;
        protected bool IsCustomerProductBusy { get; set; } = false;
        protected bool IsCustomerProductPackageBusy { get; set; } = false;
        protected bool IsCustomerProductFirstRender { get; set; } = true;
        protected bool IsCustomerProductPackageFirstRender { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.CustomerProducts, null);
        }

        protected async Task<IEnumerable<Customer>> SearchCustomers(string searchText)
        {
            var url = $"api/customers/search?columns=code&searchStrings={searchText}";
            var result = await ApiService.GetAllAsync<Customer>(url);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if(ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);
            
            return result;
        }
    }
}