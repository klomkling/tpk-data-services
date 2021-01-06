using System;
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
    public class CustomerProductComponentBase : TgComponentBase<CustomerProductWithDetail, CustomerProduct,
        CustomerProductEditContext>
    {
        protected readonly IEnumerable<string> StatusDropdown = new List<string> {"Discontinued", "For Sale"};
        private Product _selectedProduct;
        private ProductUnit _selectedProductUnit;

        protected CustomerProductWithDetail CustomerProductWithDetail { get; set; }

        protected Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (Equals(_selectedProduct, value)) return;
                _selectedProduct = value;
                DataEditContext.ProductId = value?.Id ?? 0;
                var isProductChanged = DataEditContext.ProductId != DataEditContext.DataItem.ProductId;
                DataEditContext.Code = isProductChanged ? value?.Code : DataEditContext.DataItem.Code;
                DataEditContext.Name = isProductChanged ? value?.Name : DataEditContext.DataItem.Name;
                DataEditContext.NormalPrice = isProductChanged ? value?.NormalPrice ?? 0m : DataEditContext.DataItem.NormalPrice;
                DataEditContext.MoqPrice = isProductChanged ? value?.MoqPrice ?? 0m : DataEditContext.DataItem.MoqPrice;
                DataEditContext.MinimumOrder = isProductChanged ? value?.MinimumOrder ?? 0m : DataEditContext.DataItem.MinimumOrder;
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
            await InitComponent(TgClaimTypes.CustomerProducts, "api/customer-products");
        }

        protected async Task<LoadResult> LoadCustomerProducts(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            var url = $"api/customers/{MasterId}/products";
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

        protected override void OnBeforeRowEditing(CustomerProductWithDetail model = default)
        {
            DataEditContext.IsActive = true;
            DataEditContext.CustomerId = MasterId;
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