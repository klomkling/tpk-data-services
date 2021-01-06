using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class TransportationRequestLineEditContext : EditContextBase<TransportationRequestLine>
    {
        public TransportationRequestLineEditContext() : base(null)
        {
        }

        public TransportationRequestLineEditContext(TransportationRequestLine dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            TransportationRequestId = DataItem.TransportationRequestId;
            ProductId = DataItem.ProductId;
            Quantity = DataItem.Quantity;
            ReadyQuantity = DataItem.ReadyQuantity;
            RemainQuantity = DataItem.RemainQuantity;
        }
        
        public int TransportationRequestId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReadyQuantity { get; set; }
        [NotMapped] public decimal RemainQuantity { get; set; }
        [NotMapped] public string ProductCode { get; set; }
        [NotMapped] public string ProductName { get; set; }
        [NotMapped] public string UnitName { get; set; }
    }
}