using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class SupplierOrderLine : TgModelBase
    {
        public int SupplierOrderId { get; set; }
        public int SupplierProductId { get; set; }
        public int? ProductId { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string Remark { get; set; }
    }
}