using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class ProductionInventoryReference : TgMinimalModelBase
    {
        public int ProductionOrderId { get; set; }
        public int InventoryRequestLineId { get; set; }
        public decimal Quantity { get; set; }
    }
}