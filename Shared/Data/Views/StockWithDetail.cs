using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class StockWithDetail : Stock
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public string StockroomName { get; set; }
        public string StockLocation { get; set; }
        public string PackageCode { get; set; }
        public decimal BookedQuantity { get; set; }
    }
}