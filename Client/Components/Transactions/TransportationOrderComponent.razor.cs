using System;
using System.Collections.Generic;
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
    public class TransportationOrderComponentBase : TgComponentBase<TransportationOrderWithDetail, TransportationOrder, TransportationOrderEditContext>
    {
        protected TransportationOrderWithDetail TransportationOrderWithDetail { get; set; }
        protected IEnumerable<TgOrderStatuses> OrderStatusCollection { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.TransportationOrders, "api/transportation-orders");
            OrderStatusCollection = StringEnumeration.GetAll<TgOrderStatuses>();
        }

        protected override void OnBeforeRowEditing(TransportationOrderWithDetail model = default)
        {
            if (DataEditContext.Id > 0) return;

            DataEditContext.OrderDate = DateTime.Now;
            DataEditContext.Status = TgOrderStatuses.New.Value;
            DataEditContext.StatusDate = DateTime.Now;
            DataEditContext.DueDate = DateTime.Now;
        }

        protected async Task<LoadResult> LoadTransportationOrders(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            return await LoadDataAsync(ApiUrl, loadOptions, cancellationToken);
        }
    }
}