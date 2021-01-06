using System.Threading.Tasks;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Pages.Inventories
{
    public class InventoryRequestsBase : TgMinimalComponentBase
    {
        private int _inventoryRequestId;

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

        protected bool IsInventoryRequestBusy { get; set; } = false;
        protected bool IsRequestLineBusy { get; set; } = false;
        protected int InventoryRequestLineId { get; set; } = 0;
        protected int InventoryRequestSelectedCount { get; set; } = 0;
        protected int InventoryRequestLineSelectedCount { get; set; } = 0;
        protected bool IsInventoryRequestFirstRender { get; set; } = true;
        protected bool IsRequestLineFirstRender { get; set; } = true;
        
        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.InventoryRequests, null);
        }
    }
}