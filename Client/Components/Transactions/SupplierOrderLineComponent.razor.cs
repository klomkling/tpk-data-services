using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.AspNetCore.Components;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;
using Tpk.DataServices.Shared.Data.Views;

namespace Tpk.DataServices.Client.Components.Transactions
{
    public class SupplierOrderLineComponentBase
        : TgComponentBase<SupplierOrderLineWithDetail, SupplierOrderLine, SupplierOrderLineEditContext>
    {
        private Product _selectedProduct;
        private SupplierProduct _selectedSupplierProduct;

        protected SupplierOrderLineWithDetail SupplierOrderLineWithDetail { get; set; }
        protected bool IsPermanent { get; set; }
        protected string UnitName { get; set; }

        protected SupplierProduct SelectedSupplierProduct
        {
            get => _selectedSupplierProduct;
            set
            {
                if (Equals(_selectedSupplierProduct, value)) return;
                _selectedSupplierProduct = value;
                DataEditContext.ProductId = value?.ProductId;
                DataEditContext.SupplierProductId = value?.Id ?? 0;
                var isProductChanged = DataEditContext.SupplierProductId != DataEditContext.DataItem.SupplierProductId;
                DataEditContext.Description = isProductChanged ? value?.Name : DataEditContext.DataItem.Description;
                DataEditContext.Price = isProductChanged ? value?.NormalPrice ?? 0m : DataEditContext.Price;
                LoadProduct(DataEditContext.ProductId);
                StateHasChanged();
            }
        }

        protected Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (Equals(_selectedProduct, value)) return;
                _selectedProduct = value;
                DataEditContext.ProductCode = value.Code;
                LoadProductUnit(value?.ProductUnitId ?? 0);
                
                StateHasChanged();
            }
        }

        [Parameter] public int SupplierId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.SupplierOrders, "api/supplier-order-lines");
        }

        protected override void OnBeforeRowEditing(SupplierOrderLineWithDetail model = default)
        {
            DataEditContext.SupplierOrderId = MasterId;
            SelectedSupplierProduct = null;

            if (DataEditContext.Id == 0) return;

            Task.Run(async () =>
            {
                SelectedSupplierProduct =
                    await ApiService.GetAsync<SupplierProduct>(DataEditContext.DataItem.SupplierProductId,
                        "api/supplier-products");
            });
        }

        protected override void OnSelectedCollectionChanged(IEnumerable<SupplierOrderLineWithDetail> collection)
        {
            var permanentCount = collection.Count(c => c.InventoryRequestLineId > 0);
            IsPermanent = SelectedCount == 1 && SelectedItem.InventoryRequestLineId > 0 ||
                          SelectedCount == permanentCount;
        }

        protected override bool OnValidateRowDoubleClick(SupplierOrderLineWithDetail args)
        {
            return args.InventoryRequestLineId == 0;
        }

        private void LoadProduct(int? id)
        {
            if (id == 0 || id == null)
            {
                SelectedProduct = null;
                return;
            }

            Task.Run(async () => { SelectedProduct = await ApiService.GetAsync<Product>(id, "api/products"); });
        }

        protected async Task<LoadResult> LoadSupplierOrderLines(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            var url = $"api/supplier-orders/{MasterId}/lines";

            return await LoadDataAsync(url, loadOptions, cancellationToken);
        }

        protected async Task<IEnumerable<SupplierProduct>> SearchSupplierProducts(string searchText)
        {
            var response =
                await ApiService.GetAllAsync<SupplierProduct>(
                    $"api/supplier-products/search?columns=SupplierId&searchStrings={SupplierId}&columns=code&searchStrings={searchText}");
            return response;
        }
        
        protected void OnQuantityChanged(decimal newQuantity)
        {
            if (SelectedSupplierProduct.MinimumOrder > 0m && SelectedSupplierProduct.MoqPrice > 0m)
            {
                DataEditContext.Price = newQuantity >= SelectedSupplierProduct.MinimumOrder
                    ? SelectedSupplierProduct.MoqPrice
                    : SelectedSupplierProduct.NormalPrice;
            }
            DataEditContext.EditQuantity = newQuantity;
        }
        
        private void LoadProductUnit(int id)
        {
            if (id == 0)
            {
                UnitName = null;
                return;
            }

            Task.Run(async () =>
            {
                var productUnit = await ApiService.GetAsync<ProductUnit>(id, "api/product-units");
                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

                UnitName = productUnit == null ? null : $" ({productUnit.Code})";
                StateHasChanged();
            });
        }
    }
}