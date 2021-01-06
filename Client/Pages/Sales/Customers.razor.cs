using System.Threading.Tasks;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Pages.Sales
{
    public class CustomersBase : TgMinimalComponentBase
    {
        private int _customerId;

        protected int CustomerId
        {
            get => _customerId;
            set
            {
                if (Equals(_customerId, value)) return;
                _customerId = value;
                IsContactFirstRender = true;
            }
        }
        
        protected bool IsMasterBusy { get; set; }
        protected bool IsContactBusy { get; set; }
        protected bool IsAddressBusy { get; set; }
        protected int MasterSelectedCount { get; set; } = 0;
        protected bool IsMasterFirstRender { get; set; } = true;
        protected bool IsContactFirstRender { get; set; } = true;
        protected bool IsAddressFirstRender { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.Customers, null);
        }
    }
}