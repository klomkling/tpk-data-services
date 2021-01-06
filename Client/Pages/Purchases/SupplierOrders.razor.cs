using System.Collections.Generic;
using System.Threading.Tasks;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Pages.Purchases
{
    public class SupplierOrdersBase : TgMinimalComponentBase
    {
        private Supplier _selectedSupplier;

        protected Supplier SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                if (Equals(_selectedSupplier, value)) return;  
                _selectedSupplier = value;
                if (Equals(value, null))
                {
                    SupplierOrderId = 0;
                }
                StateHasChanged();
            }
        }

        protected int SupplierOrderId { get; set; } = 0;
        protected int SupplierOrderSelectedCount { get; set; } = 0;
        protected bool IsSupplierOrderBusy { get; set; } = false;
        protected bool IsSupplierOrderLineBusy { get; set; } = false;
        protected bool IsSupplierOrderFirstRender { get; set; } = true;
        protected bool IsSupplierOrderLineFirstRender { get; set; } = true;

        protected bool RefreshOrderDataGrid { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.SupplierOrders, null);
        }

        protected async Task<IEnumerable<Supplier>> SearchSuppliers(string searchText)
        {
            // Sort A_{column name} or D_{column name}
            var url = $"api/suppliers/search?columns=code&searchStrings={searchText}";
            var result = await ApiService.GetAllAsync<Supplier>(url);
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