using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;
using Tpk.DataServices.Shared.Data.Views;

namespace Tpk.DataServices.Client.Components.Transactions
{
    public class ProductionOrderComponentBase
        : TgComponentBase<ProductionOrderWithDetail, ProductionOrder, ProductionOrderEditContext>
    {
        private Product _selectedProduct;
        private bool _isGeneratedClick;

        protected ProductionOrderWithDetail ProductionOrderWithDetail { get; set; }
        protected IEnumerable<TgOrderStatuses> OrderStatusCollection { get; set; }
        protected bool CanGenerateRequest { get; set; }
        protected bool IsFromProductionRequest { get; set; }
        protected bool CanChange { get; set; }
        protected string UnitName { get; set; }

        protected Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (Equals(_selectedProduct, value)) return;
                _selectedProduct = value;
                DataEditContext.ProductId = value?.Id ?? 0;
                DataEditContext.ProductCode = value?.Code;
                DataEditContext.ProductName = value?.Name;
                LoadProductUnit(value?.ProductUnitId ?? 0);

                StateHasChanged();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.ProductionOrders, "api/production-orders");
            OrderStatusCollection = StringEnumeration.GetAll<TgOrderStatuses>();
        }

        protected override void OnSelectedCollectionChanged(IEnumerable<ProductionOrderWithDetail> collection)
        {
            CanGenerateRequest = IsCanGenerateRequest();
        }

        protected override void OnBeforeRowEditing(ProductionOrderWithDetail model = default)
        {
            IsFromProductionRequest = (model?.InventoryRequestCount ?? 0) > 0;
            CanChange = model?.IsFromProductionRequest ?? true;
            SelectedProduct = null;

            if (DataEditContext.Id == 0)
            {
                DataEditContext.OrderDate = DateTime.Now;
                DataEditContext.DueDate = DateTime.Now;
                DataEditContext.Status = TgOrderStatuses.New.Value;
                DataEditContext.StatusDate = DateTime.Now;
                DataEditContext.LotNumber = "Waiting";
                return;
            }

            Task.Run(async () =>
            {
                var result = await ApiService.GetAsync<Product>(DataEditContext.DataItem.ProductId, "api/products");
                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

                SelectedProduct = result;
            });
        }

        protected override Task PrepareRowForUpdate()
        {
            if (DataEditContext.ReadyQuantity > 0m && DataEditContext.ReadyQuantity < DataEditContext.Quantity)
            {
                DataEditContext.Status = TgOrderStatuses.Start.Value;
            }

            if (DataEditContext.ReadyQuantity == DataEditContext.Quantity)
            {
                DataEditContext.Status = TgOrderStatuses.ReadyToPickup.Value;
            }

            return base.PrepareRowForUpdate();
        }

        protected async Task GenerateInventoryRequest()
        {
            if (SelectedCount == 0)
            {
                ToastService.ShowWarning("No order is selected");
                return;
            }

            // Check with api
            var collection = SelectedCollection.Select(c => c.Id).ToList();
            var canGenerate = await ApiService.PostAsync("api/production-orders/can-generate-requests", collection);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);
            if (canGenerate == false)
            {
                ToastService.ShowWarning("None of order is ready to generates");
                return;
            }

            if (_isGeneratedClick) return;
            _isGeneratedClick = true;

            // Select only have new production quantity
            var url = $"{ApiUrl}/generate-request";
            var done = await ApiService.PostAsync(url, collection);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            _isGeneratedClick = false;

            if (done == false) return;

            ToastService.ShowSuccess("Successfully generated delivery sheet");

            await Grid.Refresh();

            CanGenerateRequest = IsCanGenerateRequest();
            StateHasChanged();
        }

        protected async Task<LoadResult> LoadProductionOrders(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            return await LoadDataAsync(ApiUrl, loadOptions, cancellationToken);
        }

        protected Task<IEnumerable<TgOrderStatuses>> LoadStatusAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(OrderStatusCollection);
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

        private bool IsCanGenerateRequest()
        {
            return ((SelectedCollection?.Any(o => o.DeliveredQuantity < o.ReadyQuantity) ?? false) ||
                    SelectedCount == 1 && SelectedItem.InventoryRequestCount == 0) &&
                   (SelectedCollection?.Any(o =>
                       o.LotNumber.Equals("Waiting",
                           StringComparison.InvariantCultureIgnoreCase) == false) ?? false);
        }
    }
}