using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.Models;
using Tpk.DataServices.Shared.Data.Views;

namespace Tpk.DataServices.Client.Pages.Inventories
{
    public class DeliverySheetsBase : TgMinimalComponentBase
    {
        private InventoryRequest _selectedInventoryRequest;
        private bool _confirmClick;
        private bool _isSelectionProcessing;

        protected IEnumerable<TgOrderStatuses> OrderStatusCollection;
        private int _masterPageIndex = 0;

        protected InventoryRequest SelectedInventoryRequest
        {
            get => _selectedInventoryRequest;
            set
            {
                if (Equals(_selectedInventoryRequest, value)) return;
                _selectedInventoryRequest = value;
                if (Equals(value, null))
                {
                    IsInventoryRequestSelected = false;
                    StateHasChanged();
                    return;
                }

                IsInventoryRequestSelected = true;
                IsSelectionProcessing = false;
                RequestType = _selectedInventoryRequest.RequestType;
                RequestTypeName = StringEnumeration.FromValue<TgInventoryRequestTypes>(RequestType).DisplayName;
                LineFirstRender = true;
                DetailFirstRender = true;
                InventoryRequestLineId = 0;
                MasterPageIndex = 0;
                if (RequestType != null && RequestType.Equals(TgInventoryRequestTypes.CustomerOrder.Value,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    LoadCustomerOrderFromInventoryRequestId(SelectedInventoryRequest.Id);
                }

                StateHasChanged();
            }
        }

        protected bool IsSelectionProcessing
        {
            get => _isSelectionProcessing;
            set
            {
                if (Equals(_isSelectionProcessing, value)) return;
                _isSelectionProcessing = value;
                StateHasChanged();
            }
        }

        private CustomerOrderWithDetail SelectedCustomerOrder { get; set; }
        protected string CustomerName { get; set; }
        protected bool IsInventoryRequestSelected { get; set; }

        protected int MasterPageIndex
        {
            get => _masterPageIndex;
            set
            {
                if (Equals(_masterPageIndex, value)) return;
                _masterPageIndex = value;
                IsSelectionProcessing = false;
                StateHasChanged();
            }
        }

        protected bool LineFirstRender { get; set; } = true;
        protected bool DetailFirstRender { get; set; } = true;
        protected bool IsStockFirstRender { get; set; } = true;
        protected bool IsLineBusy { get; set; }
        protected bool IsDetailBusy { get; set; }
        protected int LineSelectedCount { get; set; } = 0;
        protected int DetailSelectedCount { get; set; } = 0;
        protected int InventoryRequestLineId { get; set; }

        protected string SelectionsTabCss =>
            LineSelectedCount != 1 || InventoryRequestLineId == 0 ? "disabled-tab" : null;

        protected string RequestType { get; set; }
        protected string RequestTypeName { get; set; }

        protected bool IsManualRequest => RequestType == TgInventoryRequestTypes.ManualRequest.Value ||
                                          RequestType == TgInventoryRequestTypes.ManualReturn.Value ||
                                          RequestType == TgInventoryRequestTypes.ProductionRequest.Value;

        protected bool IsCustomerOrder => RequestType == TgInventoryRequestTypes.CustomerOrder.Value;

        protected async Task<IEnumerable<InventoryRequest>> SearchInventoryRequests(string searchText)
        {
            // Sort A_{column name} or D_{column name}
            var url = "api/inventory-requests/search?columns=i_requestType&searchStrings=o_r_m" +
                      "&columns=ni_status&searchStrings=d_c_x&" +
                      $"&columns=ge_requestNumber&searchStrings={searchText}&orderColumns=d_requestNumber";
            var result = await ApiService.GetAllAsync<InventoryRequest>(url);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            return result;
        }

        private void LoadCustomerOrderFromInventoryRequestId(int id)
        {
            if (id == 0)
            {
                SelectedCustomerOrder = null;
                CustomerName = null;
                return;
            }

            var url = $"api/customer-orders/inventory-requests";
            Task.Run(async () =>
            {
                var result = await ApiService.GetAsync<CustomerOrderWithDetail>(id, url);
                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

                SelectedCustomerOrder = result;
                if (result == null)
                {
                    CustomerName = null;
                    return;
                }

                CustomerName = SelectedCustomerOrder.CustomerName;
                await InvokeAsync(StateHasChanged);
            });
        }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.InventoryRequestLineDetails, null);

            OrderStatusCollection = StringEnumeration.GetAll<TgOrderStatuses>();
        }

        protected Task SelectionProcess()
        {
            IsSelectionProcessing = true;
            StateHasChanged();
            return Task.CompletedTask;
        }

        protected async Task DoneSelectionProcess()
        {
            var result =
                await ApiService.GetAsync<InventoryRequest>(SelectedInventoryRequest.Id, "api/inventory-requests");
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);
            SelectedInventoryRequest = result;

            IsSelectionProcessing = false;
            MasterPageIndex = 0;
            StateHasChanged();
        }

        protected void OnImportCompleted()
        {
            Console.WriteLine("Import Completed");
            MasterPageIndex = 0;
        }

        protected async Task ConfirmDeliverySheet()
        {
            if (_confirmClick) return;
            _confirmClick = true;

            // Check if inventory request type is manual request 
            // var url = IsCustomerOrder
            //     ? "api/inventory-requests/generate-transportation-request"
            //     : "api/inventory-requests/confirm-delivery";
            var done = await ApiService.PostAsync("api/inventory-requests/confirm-delivery",
                new List<int> {SelectedInventoryRequest.Id});
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            if (done == false)
            {
                ToastService.ShowWarning(
                    $"Inventory Request #{SelectedInventoryRequest.RequestNumber:000000} does not ready for {(IsCustomerOrder ? "transport" : "delivery")}");
                _confirmClick = false;
                return;
            }

            ToastService.ShowSuccess(
                $"Successfully {(IsCustomerOrder ? "generate transportation request" : "confirmed")}");
            _confirmClick = false;

            SelectedInventoryRequest = null;
        }
    }
}