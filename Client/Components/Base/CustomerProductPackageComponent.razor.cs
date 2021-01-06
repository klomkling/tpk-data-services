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
    public class CustomerProductPackageComponentBase : TgComponentBase<CustomerProductPackageWithDetail,
        CustomerProductPackage, CustomerProductPackageEditContext>
    {
        private Product _selectedPackageType;
        protected CustomerProductPackageWithDetail CustomerProductPackageWithDetail { get; set; }

        protected Product SelectedPackageType
        {
            get => _selectedPackageType;
            set
            {
                if (Equals(_selectedPackageType, value)) return;
                _selectedPackageType = value;
                DataEditContext.PackageTypeId = value?.Id ?? 0;
                StateHasChanged();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.CustomerProductPackages, "api/customer-product-packages");
        }

        protected async Task<IEnumerable<Product>> SearchPackageTypes(string searchText)
        {
            var url = $"api/products/search?columns=i_inventoryTypeId&searchStrings={TgInventoryTypes.PackageTypeBags.Value}_{TgInventoryTypes.PackageTypeBoxes.Value}&columns=code&searchStrings={searchText}";
            var result = await ApiService.GetAllAsync<Product>(url);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            return result;
        }
        
        protected async Task<LoadResult> LoadCustomerProductPackages(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            var url = $"api/customer-products/{MasterId}/packages";
            return await LoadDataAsync(url, loadOptions, cancellationToken);
        }

        protected override void OnBeforeRowEditing(CustomerProductPackageWithDetail model = default)
        {
            DataEditContext.CustomerProductId = MasterId;
        }
    }
}