using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class ProductionRequest : TgModelBase
    {
        public int RequestNumber { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public string Comment { get; set; }
    }
}