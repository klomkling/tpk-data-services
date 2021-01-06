using System.Threading.Tasks;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Components.Transactions
{
    public class TransportationOrderLineComponentBase 
        : TgComponentBase<TransportationOrderLine, TransportationOrderLine, TransportationOrderLineEditContext>
    {
        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.TransportationOrders, "api/transportation-order-lines");
        }
    }
}