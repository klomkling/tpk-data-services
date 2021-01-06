using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class InventoryRequestLineWithDetail : InventoryRequestLine
    {
        public string StatusName { get; set; }
        public string SupplierProductCode { get; set; }
        public string SupplierProductName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public decimal BookedQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
        public decimal TestColumn { get; set; }
    }
}