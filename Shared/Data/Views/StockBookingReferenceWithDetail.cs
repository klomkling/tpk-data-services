using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class StockBookingReferenceWithDetail : StockBookingReference
    {
        public string CustomerName { get; set; }
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string StockroomName { get; set; }

        public string Description => string.IsNullOrEmpty(StockroomName)
            ? CustomerName
            : $"{CustomerName} to {StockroomName}";
    }
}