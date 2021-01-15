using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Pages.Inventories
{
    public class InventoryReceiptsBase : TgMinimalComponentBase
    {
        private InventoryRequest _selectedInventoryRequest;
        private bool _confirmClick;

        protected IEnumerable<TgOrderStatuses> OrderStatusCollection;

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
                RequestType = _selectedInventoryRequest.RequestType;
                RequestTypeName = StringEnumeration.FromValue<TgInventoryRequestTypes>(RequestType).DisplayName;
                LineFirstRender = true;
                DetailFirstRender = true;
                InventoryRequestLineId = 0;
                MasterPageIndex = 0;
                if (RequestType != null && RequestType.Equals(TgInventoryRequestTypes.PurchaseOrder.Value,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    LoadSupplierOrderFromInventoryRequestId(_selectedInventoryRequest.Id);
                }
                StateHasChanged();
            }
        }
        protected SupplierOrder SelectedSupplierOrder { get; set; }
        protected Supplier SelectedSupplier { get; set; }
        protected string SupplierName { get; set; }
        protected bool IsInventoryRequestSelected { get; set; }
        protected int MasterPageIndex { get; set; } = 0;
        protected bool LineFirstRender { get; set; } = true;
        protected bool DetailFirstRender { get; set; } = true;
        protected bool IsLineBusy { get; set; }
        protected bool IsDetailBusy { get; set; }
        protected int LineSelectedCount { get; set; } = 0;
        protected int DetailSelectedCount { get; set; } = 0;
        protected int InventoryRequestLineId { get; set; }
        protected string SelectionsTabCss =>
            LineSelectedCount != 1 || InventoryRequestLineId == 0 ? "disabled-tab" : null;
        protected string ImportTabCss => LineSelectedCount != 1 || InventoryRequestLineId == 0 ? "disabled-tab" : null;
        protected string RequestType { get; set; }
        protected string RequestTypeName { get; set; }

        protected async Task<IEnumerable<InventoryRequest>> SearchInventoryRequests(string searchText)
        {
            // Sort A_{column name} or D_{column name}
            var url = "api/inventory-requests/search?columns=i_requestType&searchStrings=b_p_n" +
                      "&columns=ni_status&searchStrings=c_x" +
                      $"&columns=ge_requestNumber&searchStrings={searchText}&orderColumns=a_requestNumber";
            var result = await ApiService.GetAllAsync<InventoryRequest>(url);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            return result;
        }

        private void LoadSupplierOrderFromInventoryRequestId(int id)
        {
            if (id == 0)
            {
                SelectedSupplierOrder = null;
                SelectedSupplier = null;
                SupplierName = null;
                return;
            }
            
            var url = $"api/inventory-requests/{id}/supplier-order";
            Task.Run(async () =>
            {
                var result = await ApiService.GetAsync<SupplierOrder>(url);
                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

                SelectedSupplierOrder = result;
                if (result == null)
                {
                    SelectedSupplier = null;
                    SupplierName = null;
                    return;
                }
                
                LoadSupplier(SelectedSupplierOrder.SupplierId);
            });
        }

        protected void LoadSupplier(int id)
        {
            var url = $"api/suppliers";
            Task.Run(async () =>
            {
                var result = await ApiService.GetAsync<Supplier>(id, url);
                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

                SelectedSupplier = result;
                SupplierName = result == null ? null : SelectedSupplier.Name;
                StateHasChanged();
            });
        }
        
        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.InventoryRequestLineDetails, null);

            OrderStatusCollection = StringEnumeration.GetAll<TgOrderStatuses>();
        }

        protected void OnImportCompleted()
        {
            Console.WriteLine("Import Completed");
            MasterPageIndex = 0;
        }

        protected async Task ConfirmReceipt()
        {
            if (_confirmClick) return;
            _confirmClick = true;
            
            var url = $"api/inventory-requests/confirm-receipt";

            var done = await ApiService.PostAsync(url, new List<int> {SelectedInventoryRequest.Id});
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            if (done == false) return;

            ToastService.ShowSuccess("Successfully confirmed");
            _confirmClick = false;

            SelectedInventoryRequest = null;
            
            await Task.CompletedTask;
        }
    }
}