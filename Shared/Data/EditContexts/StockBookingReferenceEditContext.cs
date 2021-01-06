using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class StockBookingReferenceEditContext : EditContextBase<StockBookingReference>
    {
        public StockBookingReferenceEditContext() : base(null)
        {
        }

        public StockBookingReferenceEditContext(StockBookingReference dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            ProductId = DataItem.ProductId;
            InventoryRequestLineId = DataItem.InventoryRequestLineId;
            Quantity = DataItem.Quantity;
            DueDate = DataItem.DueDate;
            CompletedDate = DataItem.CompletedDate;
        }
        
        public int ProductId { get; set; }
        public int InventoryRequestLineId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
}