using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class InventoryRequestLine : TgModelBase
    {
        public int InventoryRequestId { get; set; }
        public int? SupplierProductId { get; set; }
        public int? ProductId { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReadyQuantity { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        [NotMapped] public decimal RemainQuantity => Quantity - ReadyQuantity;
    }
}