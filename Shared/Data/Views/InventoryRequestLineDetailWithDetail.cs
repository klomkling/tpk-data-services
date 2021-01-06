using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class InventoryRequestLineDetailWithDetail : InventoryRequestLineDetail
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string StockroomName { get; set; }
        public string StockLocationName { get; set; }
        public string PackageTypeCode { get; set; }
        public DateTime ReceivedDate { get; set; }
    }
}