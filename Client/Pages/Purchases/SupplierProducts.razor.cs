using System.Collections.Generic;
using System.Threading.Tasks;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Pages.Purchases
{
    public class SupplierProductsBase : TgMinimalComponentBase
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
                    SupplierProductId = 0;
                }
                StateHasChanged();
            }
        }

        protected int SupplierProductId { get; set; } = 0;
        protected int SupplierProductSelectedCount { get; set; } = 0;
        protected bool IsSupplierProductBusy { get; set; } = false;
        protected bool IsSupplierProductPackageBusy { get; set; } = false;
        protected bool IsSupplierProductFirstRender { get; set; } = true;
        protected bool IsSupplierProductPackageFirstRender { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.SupplierProducts, null);
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
    }
}