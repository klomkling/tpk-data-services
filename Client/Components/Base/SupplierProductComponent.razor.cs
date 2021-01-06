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
        SupplierProductComponentBase : TgComponentBase<SupplierProductWithDetail, SupplierProduct,
            SupplierProductEditContext>
    {
        protected readonly IEnumerable<string> StatusDropdown = new List<string> {"Discontinued", "Active"};
        protected readonly IEnumerable<string> VatDropdown = new List<string> {"Included", "Excluded"};
        private Product _selectedProduct;
        private ProductUnit _selectedProductUnit;

        protected SupplierProductWithDetail SupplierProductWithDetail { get; set; }

        protected Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (Equals(_selectedProduct, value)) return;
                _selectedProduct = value;
                DataEditContext.ProductId = value?.Id ?? 0;
                LoadProductUnit(value?.ProductUnitId ?? 0);
                StateHasChanged();
            }
        }

        public ProductUnit SelectedProductUnit
        {
            get => _selectedProductUnit;
            set
            {
                if (Equals(_selectedProductUnit, value)) return;
                _selectedProductUnit = value;
                DataEditContext.ProductUnitId = value?.Id ?? 0;
                StateHasChanged();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.SupplierProducts, "api/supplier-products");
        }

        protected async Task<LoadResult> LoadSupplierProducts(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            var url = $"api/suppliers/{MasterId}/products";
            return await LoadDataAsync(url, loadOptions, cancellationToken);
        }

        protected async Task<IEnumerable<Product>> SearchProducts(string searchText)
        {
            var url = $"api/products/search?columns=code&searchStrings={searchText}";
            return await ApiService.GetAllAsync<Product>(url);
        }

        private void LoadProductUnit(int id)
        {
            if (id == 0)
            {
                SelectedProductUnit = null;
                return;
            }

            Task.Run(async () =>
            {
                SelectedProductUnit = await ApiService.GetAsync<ProductUnit>(id, "api/product-units");
            });
        }

        protected async Task<IEnumerable<ProductUnit>> SearchProductUnits(string searchText)
        {
            var url = $"api/product-units/search?columns=code&searchStrings={searchText}";
            return await ApiService.GetAllAsync<ProductUnit>(url);
        }

        protected override void OnBeforeRowEditing(SupplierProductWithDetail model = default)
        {
            DataEditContext.IsActive = true;
            DataEditContext.SupplierId = MasterId;
            if (DataEditContext.Id == 0) return;

            SelectedProduct = null;

            if (DataEditContext.DataItem?.ProductId > 0)
            {
                Task.Run(async () =>
                {
                    SelectedProduct =
                        await ApiService.GetAsync<Product>(DataEditContext.DataItem.ProductId, "api/products");
                });
            }
        }

        protected override Task PrepareRowForUpdate()
        {
            if (SelectedProduct != null && DataEditContext.ProductUnitId != SelectedProduct.ProductUnitId)
            {
                DataEditContext.ProductUnitId = SelectedProduct.ProductUnitId;
            }

            DataEditContext.UpdateDataItem();

            return Task.CompletedTask;
        }
    }
}