using System.Threading.Tasks;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Pages.Purchases
{
    public class SuppliersBase : TgMinimalComponentBase
    {
        private int _supplierId;

        protected int SupplierId
        {
            get => _supplierId;
            set
            {
                if (Equals(_supplierId, value)) return;
                _supplierId = value;
                IsContactFirstRender = true;
            }
        }

        protected bool IsMasterBusy { get; set; } = false;
        protected bool IsContactBusy { get; set; } = false;
        protected int MasterSelectedCount { get; set; } = 0;
        protected bool IsMasterFirstRender { get; set; } = true;
        protected bool IsContactFirstRender { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.Suppliers, null);
        }
    }
}