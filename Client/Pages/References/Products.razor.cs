using System.Threading.Tasks;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Pages.References
{
    public class ProductsBase : TgMinimalComponentBase
    {
        private int _productId;
        private int _stockId;

        protected int ProductId
        {
            get => _productId;
            set
            {
                if (Equals(_productId, value)) return;
                _productId = value;
                StockId = 0;
                IsMasterFirstRender = true;
                IsPackageFirstRender = true;
                IsStockFirstRender = true;
                IsBookingFirstRender = true;
            }
        }

        protected int StockId
        {
            get => _stockId;
            set
            {
                if (Equals(_stockId, value)) return;
                _stockId = value;
                IsBookingFirstRender = true;
            }
        }

        protected bool IsBusy { get; set; }
        protected bool IsPackageBusy { get; set; }
        protected bool IsMasterFirstRender { get; set; } = true;
        protected bool IsPackageFirstRender { get; set; } = true;
        protected bool IsStockFirstRender { get; set; } = true;
        protected bool IsBookingFirstRender { get; set; } = true;
        protected bool IsProducingFirstRender { get; set; } = true;
        protected int DetailTabIndex { get; set; } = 0;
        protected int StockSelectedCount { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.Products, null);
        }
    }
}