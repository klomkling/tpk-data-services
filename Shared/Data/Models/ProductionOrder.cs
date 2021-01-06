using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class ProductionOrder : TgModelBase
    {
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string LotNumber { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReadyQuantity { get; set; }
        public string Comment { get; set; }
    }
}