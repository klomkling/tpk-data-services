using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class SupplierOrderLineWithDetail : SupplierOrderLine
    {
        public string SupplierProductCode { get; set; }
        public string SupplierProductName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public int InventoryRequestLineId { get; set; }
    }
}