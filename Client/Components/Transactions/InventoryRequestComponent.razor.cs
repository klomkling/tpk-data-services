using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;
using Tpk.DataServices.Shared.Data.Views;

namespace Tpk.DataServices.Client.Components.Transactions
{
    public class InventoryRequestComponentBase
        : TgComponentBase<InventoryRequestWithDetail, InventoryRequest, InventoryRequestEditContext>
    {
        private Stockroom _selectedStockroom;
        protected InventoryRequestWithDetail InventoryRequestWithDetail { get; set; }
        protected IEnumerable<TgOrderStatuses> RequestStatusCollection { get; set; }
        protected IEnumerable<TgInventoryRequestTypes> RequestTypeCollection { get; set; }

        protected Stockroom SelectedStockroom
        {
            get => _selectedStockroom;
            set
            {
                if (Equals(_selectedStockroom, value)) return;
                _selectedStockroom = value;
                DataEditContext.StockroomId = value?.Id ?? 0;
                StateHasChanged();
            }
        }
        protected bool IsManual => DataEditContext.RequestType.Equals(TgInventoryRequestTypes.ManualRequest.Value,
                                       StringComparison.InvariantCultureIgnoreCase) ||
                                   DataEditContext.RequestType.Equals(TgInventoryRequestTypes.ManualReturn.Value,
                                       StringComparison.InvariantCultureIgnoreCase);

        protected string RequestTypeName { get; set; }

        [Parameter] public string OnlyRequestType { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.InventoryRequests, "api/inventory-requests");

            RequestStatusCollection = StringEnumeration.GetAll<TgOrderStatuses>();
            RequestTypeCollection = StringEnumeration.GetAll<TgInventoryRequestTypes>(new[] {"M", "N"}, true);

            if (string.IsNullOrEmpty(OnlyRequestType) == false)
            {
                await LoadStockroom();
            }
        }

        protected override void OnBeforeRowEditing(InventoryRequestWithDetail model = default)
        {
            try
            {
                TgInventoryRequestTypes type;
                if (DataEditContext.Id != 0)
                {
                    type = StringEnumeration.FromValue<TgInventoryRequestTypes>(DataEditContext.DataItem.RequestType);
                    RequestTypeName = type.DisplayName;
                    return;
                }

                DataEditContext.RequestDate = DateTime.Today;
                DataEditContext.DueDate = DateTime.Today;
                DataEditContext.Status = TgOrderStatuses.New.Value;
                DataEditContext.StatusDate = DateTime.Today;
                DataEditContext.RequestedBy = UserService.Username;
                DataEditContext.RequestType = string.IsNullOrEmpty(OnlyRequestType)
                    ? TgInventoryRequestTypes.ManualRequest.Value
                    : OnlyRequestType;
                type = StringEnumeration.FromValue<TgInventoryRequestTypes>(DataEditContext.RequestType);
                RequestTypeName = type.DisplayName;

                if (SelectedStockroom != null)
                {
                    DataEditContext.StockroomId = SelectedStockroom.Id;
                }
                
                Console.WriteLine($"Stockroom Id = {DataEditContext.StockroomId}");
                StateHasChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        protected override async Task OnAfterInserted()
        {
            if (NewInsertedOrderId == 0) return;

            var url = $"{ApiUrl}/search?columns=id&searchStrings={NewInsertedOrderId}";

            var collection = await ApiService.GetAllAsync<InventoryRequestWithDetail>(url);
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();

            var selection = JsonConvert.SerializeObject(collection);
            await LocalStorageService.SetItemAsync(SavedCollectionName, selection);
        }

        protected async Task<LoadResult> LoadInventoryRequests(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            var url = ApiUrl;
            if (string.IsNullOrEmpty(OnlyRequestType) == false)
            {
                url = $"{url}/only-request-types/{OnlyRequestType}";
            }

            return await LoadDataAsync(url, loadOptions, cancellationToken);
        }

        protected async Task LoadStockroom()
        {
            var result =
                await ApiService.GetAllAsync<Stockroom>("api/stockrooms/search?columns=name&searchStrings=productions");
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            SelectedStockroom = result?.FirstOrDefault();
        }
        
        protected async Task<IEnumerable<Stockroom>> SearchStockrooms(string searchText)
        {
            var url = $"api/stockrooms/search?columns=name&searchStrings={searchText}";
            var result = await ApiService.GetAllAsync<Stockroom>(url);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            return result;
        }

        protected bool IsRemarkReadOnly()
        {
            return DataEditContext.RequestType.Equals(TgInventoryRequestTypes.ManualRequest.Value,
                       StringComparison.InvariantCultureIgnoreCase) == false &&
                   DataEditContext.RequestType.Equals(TgInventoryRequestTypes.ManualReturn.Value,
                       StringComparison.InvariantCultureIgnoreCase) == false;
        }
    }
}