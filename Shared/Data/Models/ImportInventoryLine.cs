using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class ImportInventoryLine
    {
        public int InventoryRequestLineId { get; set; }
        public int StockroomId { get; set; }
        public string StockroomName { get; set; }
        public int StockLocationId { get; set; }
        public string StockLocation { get; set; }
        public int PackageTypeId { get; set; }
        public string PackageCode { get; set; }
        public string PalletNo { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public int PackageNumber { get; set; }
        public decimal Quantity { get; set; }
        public string LotNumber { get; set; }
        public string Reason { get; set; }
        
        [NotMapped]
        public bool IsValid => string.IsNullOrEmpty(Reason);
    }
}