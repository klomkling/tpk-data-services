using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class StockBookingDetailReference : TgMinimalModelBase
    {
        public int StockId { get; set; }
        public int InventoryRequestLineDetailId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
}