using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Newtonsoft.Json;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;
using Tpk.DataServices.Shared.Data.Views;

namespace Tpk.DataServices.Client.Components.Transactions
{
    public class SupplierOrderComponentBase
        : TgComponentBase<SupplierOrderWithDetail, SupplierOrder, SupplierOrderEditContext>
    {
        private bool _isGeneratedClick;

        protected IEnumerable<TgOrderStatuses> OrderStatusCollection;
        private decimal VatRate { get; set; }

        protected SupplierOrderWithDetail SupplierOrderWithDetail { get; set; }
        protected bool CanGenerateRequest { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.SupplierOrders, "api/supplier-orders");

            OrderStatusCollection = StringEnumeration.GetAll<TgOrderStatuses>();
            await LoadVat();
        }

        protected override void OnBeforeRowEditing(SupplierOrderWithDetail model = default)
        {
            DataEditContext.SupplierId = MasterId;
            if (DataEditContext.Id != 0) return;

            DataEditContext.OrderDate = DateTime.Today;
            DataEditContext.DueDate = DateTime.Today;
            DataEditContext.Status = TgOrderStatuses.New.Value;
            DataEditContext.StatusDate = DateTime.Today;
            DataEditContext.VatRate = VatRate;
        }

        protected override async Task OnAfterInserted()
        {
            if (NewInsertedOrderId == 0) return;

            var url = $"{ApiUrl}/search?columns=id&searchStrings={NewInsertedOrderId}";

            var collection = await ApiService.GetAllAsync<SupplierOrderWithDetail>(url);
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();

            var selection = JsonConvert.SerializeObject(collection);
            await LocalStorageService.SetItemAsync(SavedCollectionName, selection);
        }

        protected override void OnSelectedCollectionChanged(IEnumerable<SupplierOrderWithDetail> collection)
        {
            CanGenerateRequest = SelectedCollection?.Any(c => c.CanGenerate) ?? false;
            StateHasChanged();
        }

        protected async Task<LoadResult> LoadSupplierOrders(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            var url = $"api/suppliers/{MasterId}/orders";

            return await LoadDataAsync(url, loadOptions, cancellationToken);
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
            var canGenerate = await ApiService.PostAsync("api/supplier-orders/can-generate-requests", collection);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);
            if (canGenerate == false)
            {
                ToastService.ShowWarning("None of order is ready to generates");
                return;
            }
            
            if (_isGeneratedClick) return;
            _isGeneratedClick = true;

            var url = $"{ApiUrl}/generate-request";
            var done = await ApiService.PostAsync(url, collection);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            _isGeneratedClick = false;

            if (done == false) return;

            ToastService.ShowSuccess("Successfully generated inventory request");

            await Grid.Refresh();

            CanGenerateRequest = SelectedCollection?.Any(c => c.CanGenerate) ?? false;;
            StateHasChanged();
        }

        private async Task LoadVat()
        {
            var url = "api/system-options/search?columns=name&searchStrings=vat rate";

            var collection = await ApiService.GetAllAsync<SystemOption>(url);
            var option = collection.FirstOrDefault();
            if (option == null) return;

            if (decimal.TryParse(option.Value, out var vat) == false) vat = 0m;

            VatRate = vat;
        }
    }
}