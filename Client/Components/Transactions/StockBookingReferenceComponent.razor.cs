using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;
using Tpk.DataServices.Shared.Data.Views;

namespace Tpk.DataServices.Client.Components.Transactions
{
    public class StockBookingReferenceComponentBase : TgComponentBase<StockBookingReferenceWithDetail, StockBookingReference, StockBookingReferenceEditContext>
    {
        protected StockBookingReferenceWithDetail StockBookingReferenceWithDetail { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.Products, null);
        }

        protected async Task<LoadResult> LoadBookings(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            var url = $"api/products/{MasterId}/bookings";

            return await LoadDataAsync(url, loadOptions, cancellationToken);
        }
    }
}