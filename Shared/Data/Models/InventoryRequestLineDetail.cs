using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class InventoryRequestLineDetail : TgModelBase
    {
        public int InventoryRequestLineId { get; set; }
        public int ProductId { get; set; }
        public int StockroomId { get; set; }
        public int StockLocationId { get; set; }
        public int? PackageTypeId { get; set; }
        public string LotNumber { get; set; }
        public int PackageNumber { get; set; }
        public string PalletNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal ConfirmQuantity { get; set; }
        public bool IsImported { get; set; }
    }
}