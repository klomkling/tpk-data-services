using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class StockBookingReference : TgMinimalModelBase
    {
        public int ProductId { get; set; }
        public int InventoryRequestLineId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
}