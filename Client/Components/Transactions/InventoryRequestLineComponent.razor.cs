using System;
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
    public class InventoryRequestLineComponentBase
        : TgComponentBase<InventoryRequestLineWithDetail, InventoryRequestLine, InventoryRequestLineEditContext>
    {
        private InventoryRequest _inventoryRequest;
        private bool _savedCanEditValue;
        private Product _selectedProduct;
        private SupplierProduct _selectedSupplierProduct;
        private bool _supplierProductColumnVisible;

        protected InventoryRequestLineWithDetail InventoryRequestLineWithDetail { get; set; }
        protected IEnumerable<TgOrderStatuses> RequestStatusCollection { get; set; }
        protected string UnitName { get; set; }

        private InventoryRequest InventoryRequest
        {
            get => _inventoryRequest;
            set
            {
                if (Equals(_inventoryRequest, value)) return;
                _inventoryRequest = value;
                var isManualType = _inventoryRequest.RequestType == TgInventoryRequestTypes.ManualRequest.Value ||
                                   _inventoryRequest.RequestType == TgInventoryRequestTypes.ManualReturn.Value;
                var isProductionRequest =
                    _inventoryRequest.RequestType == TgInventoryRequestTypes.ProductionRequest.Value;
                IsSupplierCodeColumnVisible =
                    _inventoryRequest.RequestType == TgInventoryRequestTypes.PurchaseOrder.Value;
                IsCustomerOrder = _inventoryRequest.RequestType == TgInventoryRequestTypes.CustomerOrder.Value;
                
                StateHasChanged();
            }
        }

        protected bool IsSupplierCodeColumnVisible { get; set; }

        protected SupplierProduct SelectedSupplierProduct
        {
            get => _selectedSupplierProduct;
            set
            {
                if (Equals(_selectedSupplierProduct, value)) return;
                _selectedSupplierProduct = value;
                DataEditContext.SupplierProductId = value?.Id;
                if (DataEditContext.Id == 0) DataEditContext.Description = value?.Name;

                LoadProductUnit(value?.ProductUnitId ?? 0);

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
                DataEditContext.ProductId = value?.Id;
                DataEditContext.Description = value?.Name;

                LoadProductUnit(value?.ProductUnitId ?? 0);

                StateHasChanged();
            }
        }

        [Parameter] public bool IsCustomerOrder { get; set; }
        [Parameter] public bool IsCreatableComponent { get; set; }

        [Parameter]
        public bool SupplierProductColumnVisible
        {
            get => _supplierProductColumnVisible;
            set
            {
                if (Equals(_supplierProductColumnVisible, value)) return;
                _supplierProductColumnVisible = value;
                IsSupplierCodeColumnVisible = value;
                StateHasChanged();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.InventoryRequests, "api/inventory-request-lines");
            _savedCanEditValue = CanEdit;

            await LoadInventoryRequest();
            
            RequestStatusCollection = StringEnumeration.GetAll<TgOrderStatuses>();
        }

        protected override void OnBeforeRowEditing(InventoryRequestLineWithDetail model = default)
        {
            DataEditContext.InventoryRequestId = MasterId;
            SelectedSupplierProduct = null;
            SelectedProduct = null;

            if (DataEditContext.Id == 0)
            {
                DataEditContext.Status = TgOrderStatuses.New.Value;
                DataEditContext.StatusDate = DateTime.Today;
            }

            if (DataEditContext.DataItem?.SupplierProductId > 0)
                Task.Run(async () =>
                {
                    SelectedSupplierProduct =
                        await ApiService.GetAsync<SupplierProduct>(DataEditContext.DataItem.SupplierProductId,
                            "api/supplier-products");
                });

            if (DataEditContext.DataItem?.ProductId > 0)
                Task.Run(async () =>
                {
                    SelectedProduct =
                        await ApiService.GetAsync<Product>(DataEditContext.DataItem.ProductId, "api/products");
                });
        }

        protected async Task<LoadResult> LoadInventoryRequestLines(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            var url = $"api/inventory-requests/{MasterId}/lines";

            return await LoadDataAsync(url, loadOptions, cancellationToken);
        }

        private async Task LoadInventoryRequest()
        {
            var result = await ApiService.GetAsync<InventoryRequest>(MasterId, "api/inventory-requests");
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            InventoryRequest = result;
            
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
                UnitName = null;
                return;
            }

            Task.Run(async () =>
            {
                var productUnit = await ApiService.GetAsync<ProductUnit>(id, "api/product-units");
                UnitName = productUnit == null ? null : $" ({productUnit.Code})";
                StateHasChanged();
            });
        }
    }
}