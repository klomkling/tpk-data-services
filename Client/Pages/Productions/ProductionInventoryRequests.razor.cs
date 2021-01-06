using System;
using System.Threading.Tasks;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.Models;
using Tpk.DataServices.Shared.Data.Views;

namespace Tpk.DataServices.Client.Pages.Productions
{
    public class ProductionInventoryRequestsBase : TgMinimalComponentBase
    {
        private InventoryRequestWithDetail _selectedInventoryRequest;
        private int _inventoryRequestId;
        private int _requestLineId;

        protected InventoryRequestWithDetail SelectedInventoryRequest
        {
            get => _selectedInventoryRequest;
            set
            {
                if (Equals(_selectedInventoryRequest, value)) return;
                _selectedInventoryRequest = value;
                IsReadOnly = _selectedInventoryRequest.Status != TgOrderStatuses.New.Value;
                StateHasChanged();
            }
        }
        
        protected int InventoryRequestId
        {
            get => _inventoryRequestId;
            set
            {
                if (Equals(_inventoryRequestId, value)) return;
                _inventoryRequestId = value;
                IsRequestLineFirstRender = true;
            }
        }

        protected int RequestLineId
        {
            get => _requestLineId;
            set
            {
                if (Equals(_requestLineId, value)) return;
                _requestLineId = value;
                IsRequestDetailFirstRender = true;
            }
        }

        protected bool IsReadOnly { get; set; }
        protected bool IsMasterBusy { get; set; }
        protected bool IsRequestLineBusy { get; set; }
        protected bool IsRequestDetailBusy { get; set; }

        protected int InventoryRequestLineId { get; set; }
        protected int MasterSelectedCount { get; set; } = 0;
        protected int RequestLineSelectedCount { get; set; } = 0;
        protected int DetailPageIndex { get; set; } = 0;

        protected bool IsMasterFirstRender { get; set; } = true;
        protected bool IsRequestLineFirstRender { get; set; } = true;
        protected bool IsRequestDetailFirstRender { get; set; } = true;

        protected string SelectionsTabCss =>
            RequestLineSelectedCount != 1 || InventoryRequestLineId == 0 ? "disabled-tab" : null;
        protected string ImportTabCss => RequestLineSelectedCount != 1 || InventoryRequestLineId == 0 ? "disabled-tab" : null;

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.ProductionRequests, null);
        }
        
        protected void OnImportCompleted()
        {
            Console.WriteLine("Import Completed");
            DetailPageIndex = 0;
        }


    }
}