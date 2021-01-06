using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class Stock : TgModelBase
    {
        public DateTime ReceivedDate { get; set; }
        public int ProductId { get; set; }
        public int StockroomId { get; set; }
        public int StockLocationId { get; set; }
        public int? PackageTypeId { get; set; }
        public string LotNumber { get; set; }
        public int PackageNumber { get; set; }
        public string PalletNo { get; set; }
        public string PackingReference { get; set; }
        public decimal Quantity { get; set; }
        public decimal UsedQuantity { get; set; }
        public decimal Cost { get; set; }
        public bool IsFragment { get; set; }
        public string Remark { get; set; }

        [NotMapped] public decimal Balance { get; set; }
        [NotMapped] public decimal AvailableQuantity { get; set; }
    }
}