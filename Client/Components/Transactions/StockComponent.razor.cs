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
    public class StockComponentBase : TgComponentBase<StockWithDetail, Stock, StockEditContext>
    {
        private bool _doProcessClicked;

        protected StockWithDetail StockWithDetail { get; set; }

        [Parameter] public bool IsStockSelectionMode { get; set; }
        [Parameter] public string InventoryRequestType { get; set; }
        [Parameter] public EventCallback AfterBookingStock { get; set; }

        [Parameter]
        public int InventoryRequestLineId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.Stocks, "api/stocks");
        }

        protected async Task<LoadResult> LoadStocks(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            string url;
            if (IsStockSelectionMode && InventoryRequestType == TgInventoryRequestTypes.CustomerOrder.Value)
            {
                // Only customer order
                url = $"api/inventory-request-lines/{InventoryRequestLineId}/customer-product";
                var customerProduct = await ApiService.GetAsync<CustomerProduct>(url);
                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

                if (customerProduct != null)
                {
                    url = $"api/customer-products/{customerProduct.Id}/stocks";
                    return await LoadDataAsync(url, loadOptions, cancellationToken);
                }
            }

            if (MasterId > 0)
            {
                url = $"api/products/{MasterId}/stocks";
            }
            else
            {
                var inventoryRequestLine = await LoadInventoryRequestLine(InventoryRequestLineId);
                url = $"api/products/{inventoryRequestLine.ProductId}/stocks";
            }
            
            return await LoadDataAsync(url, loadOptions, cancellationToken);
        }

        protected async Task CancelBookingProcess()
        {
            await AfterBookingStock.InvokeAsync(default);
        }
        
        protected async Task DoBookingProcess()
        {
            if (SelectedCount == 0)
            {
                ToastService.ShowWarning("No stock is selected");
                return;
            }
            
            if (_doProcessClicked) return;
            _doProcessClicked = true;

            // Select only not yet generated order
            var collection = SelectedCollection
                .Select(c => c.Id);

            var url = $"api/inventory-request-lines/{InventoryRequestLineId}/bookings";
            var done = await ApiService.PostAsync(url, collection);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            _doProcessClicked = false;

            if (done == false)
            {
                await AfterBookingStock.InvokeAsync(default);
                return;
            }

            ToastService.ShowSuccess("Successfully booking selected stocks");
            await AfterBookingStock.InvokeAsync(default);
        }

        private async Task<InventoryRequestLine> LoadInventoryRequestLine(int id)
        {
            if (id == 0) return null;

            var result = await ApiService.GetAsync<InventoryRequestLine>(id, "api/inventory-request-lines");
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);
            
            return result;
        }
    }
}