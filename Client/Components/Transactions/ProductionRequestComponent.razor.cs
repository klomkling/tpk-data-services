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
    public class ProductionRequestComponentBase : TgComponentBase<ProductionRequestWithDetail, ProductionRequest,
        ProductionRequestEditContext>
    {
        private Product _selectedProduct;
        private bool _isGeneratedClick;

        protected ProductionRequestWithDetail ProductionRequestWithDetail { get; set; }
        protected IEnumerable<TgOrderStatuses> OrderStatusCollection { get; set; }
        protected bool CanGenerateOrder { get; set; }
        protected bool IsReadOnly { get; set; }
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
            await InitComponent(TgClaimTypes.ProductionRequests, "api/production-requests");
            OrderStatusCollection = StringEnumeration.GetAll<TgOrderStatuses>();
        }

        protected override void OnSelectedCollectionChanged(IEnumerable<ProductionRequestWithDetail> collection)
        {
            CanGenerateOrder = SelectedCollection?.Any(c => c.IsGenerated == false) ?? false;
            StateHasChanged();
        }

        protected override void OnBeforeRowEditing(ProductionRequestWithDetail model = default)
        {
            IsReadOnly = (model?.IsGenerated ?? false) || (model?.IsCustomerOrder ?? false);
            SelectedProduct = null;

            if (DataEditContext.Id == 0)
            {
                DataEditContext.RequestDate = DateTime.Now;
                DataEditContext.DueDate = DateTime.Now;
                DataEditContext.Status = TgOrderStatuses.New.Value;
                DataEditContext.StatusDate = DateTime.Now;
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

        protected async Task GenerateProductionOrder()
        {
            if (SelectedCount == 0)
            {
                ToastService.ShowWarning("No request is selected");
                return;
            }

            if (_isGeneratedClick) return;
            _isGeneratedClick = true;

            // Select only not yet generated request
            var collection = SelectedCollection
                .Where(c => c.IsGenerated == false)
                .Select(c => c.Id);
            var url = $"{ApiUrl}/generate-order";
            
            var done = await ApiService.PostAsync(url, collection);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            _isGeneratedClick = false;

            if (done == false) return;

            ToastService.ShowSuccess("Successfully generated production order");

            await Grid.Refresh();

            CanGenerateOrder = false;
            StateHasChanged();
        }

        protected async Task<LoadResult> LoadProductionRequests(DataSourceLoadOptionsBase loadOptions,
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
    }
}