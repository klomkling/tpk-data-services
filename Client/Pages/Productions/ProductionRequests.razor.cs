using System.Threading.Tasks;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Pages.Productions
{
    public class ProductionRequestsBase : TgMinimalComponentBase
    {
        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.ProductionRequests, null);
        }
    }
}