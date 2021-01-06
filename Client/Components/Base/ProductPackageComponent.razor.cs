using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Classes;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;
using Tpk.DataServices.Shared.Data.Views;

namespace Tpk.DataServices.Client.Components.Base
{
    public class
        ProductPackageComponentBase : TgComponentBase<ProductPackageWithDetail, ProductPackage, ProductPackageEditContext>
    {
        private Product _selectedPackageType;

        protected ProductPackageWithDetail ProductPackageWithDetail { get; set; }

        public Product SelectedPackageType
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
            await InitComponent(TgClaimTypes.ProductPackages, "api/product-packages");
        }

        protected override void OnBeforeRowEditing(ProductPackageWithDetail model = default)
        {
            DataEditContext.ProductId = MasterId;

            SelectedPackageType = null;

            if (DataEditContext.Id == 0) return;

            Task.Run(async () =>
            {
                SelectedPackageType =
                    await ApiService.GetAsync<Product>(DataEditContext.DataItem.PackageTypeId, "api/products");
            });
        }

        protected override Task PrepareRowForUpdate()
        {
            DataEditContext.UpdateDataItem();
            DataEditContext.DataItem.PackageTypeId = SelectedPackageType.Id;

            return Task.CompletedTask;
        }

        protected async Task<LoadResult> LoadProductPackages(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            var url = $"api/products/{MasterId}/packages";
            return await LoadDataAsync(url, loadOptions, cancellationToken);
        }

        protected async Task<IEnumerable<Product>> SearchPackageTypes(string searchText)
        {
            var url = $"api/products/search?columns=i_inventoryTypeId&searchStrings={TgInventoryTypes.PackageTypeBags.Value}_{TgInventoryTypes.PackageTypeBoxes.Value}&columns=code&searchStrings={searchText}";
            var response = await ApiService.GetAllAsync<Product>(url);
            return response;
        }
    }
}