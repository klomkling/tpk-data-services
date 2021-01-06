using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class TransportationRequestLine : TgModelBase
    {
        public int TransportationRequestId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReadyQuantity { get; set; }
        
        [NotMapped] public decimal RemainQuantity { get; set; }
    }
}