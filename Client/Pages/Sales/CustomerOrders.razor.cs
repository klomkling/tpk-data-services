using System.Collections.Generic;
using System.Threading.Tasks;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Pages.Sales
{
    public class CustomerOrdersBase : TgMinimalComponentBase
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
                    CustomerOrderId = 0;
                }
                StateHasChanged();
            }
        }

        protected int CustomerOrderId { get; set; } = 0;
        protected int CustomerOrderSelectedCount { get; set; } = 0;
        protected bool IsCustomerOrderBusy { get; set; } = false;
        protected bool IsCustomerOrderLineBusy { get; set; } = false;
        protected bool IsCustomerOrderFirstRender { get; set; } = true;
        protected bool IsCustomerOrderLineFirstRender { get; set; } = true;

        protected bool RefreshOrderDataGrid { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.CustomerOrders, null);
        }

        protected async Task<IEnumerable<Customer>> SearchCustomers(string searchText)
        {
            var url = $"api/customers/search?columns=code&searchStrings={searchText}";
            var result = await ApiService.GetAllAsync<Customer>(url);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            return result;
        }

        protected Task OrderDetailChanged(bool neededRefresh)
        {
            RefreshOrderDataGrid = neededRefresh;

            return Task.CompletedTask;
        }
    }
}