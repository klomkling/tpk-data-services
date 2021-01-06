using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class ProductionOrderWithDetail : ProductionOrder
    {
        public string StatusName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public int InventoryRequestCount { get; set; }
        public bool IsFromProductionRequest { get; set; }
        public decimal DeliveredQuantity { get; set; }
        public decimal RemainQuantity { get; set; }
        public decimal ProducingQuantity { get; set; }
    }
}