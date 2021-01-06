using System.Collections.Generic;
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
    public class TransportationRequestLineComponentBase : TgComponentBase<TransportationRequestLineWithDetail,
        TransportationRequestLine, TransportationRequestLineEditContext>
    {
        private Product _selectedProduct;

        protected TransportationRequestLineWithDetail TransportationRequestLineWithDetail { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.TransportationRequests, "api/transportation-request-lines");
        }

        protected override void OnBeforeRowEditing(TransportationRequestLineWithDetail model = default)
        {
            DataEditContext.TransportationRequestId = MasterId;
            SelectedProduct = null;

            if (DataEditContext.Id == 0)
            {
                return;
            }

            Task.Run(async () =>
            {
                SelectedProduct =
                    await ApiService.GetAsync<Product>(DataEditContext.DataItem.ProductId, "api/products");
            });
        }

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

        protected async Task<LoadResult> LoadTransportationRequestLines(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            var url = $"api/transportation-requests/{MasterId}/lines";

            return await LoadDataAsync(url, loadOptions, cancellationToken);
        }

        protected async Task<IEnumerable<Product>> SearchProducts(string searchText)
        {
            var response =
                await ApiService.GetAllAsync<Product>($"api/products/search?columns=code&searchStrings={searchText}");
            return response;
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
        
        private void LoadProductUnit(int id)
        {
            if (id == 0)
            {
                DataEditContext.UnitName = null;
                return;
            }

            Task.Run(async () =>
            {
                var productUnit = await ApiService.GetAsync<ProductUnit>(id, "api/product-units");
                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

                DataEditContext.UnitName = productUnit == null ? null : $" ({productUnit.Code})";
                StateHasChanged();
            });
        }
    }
}