using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class CustomerOrderLineWithDetail : CustomerOrderLine
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public int PackageTypeId { get; set; }
        public decimal PackageQuantity { get; set; }
        public int InventoryRequestLineId { get; set; }
    }
}