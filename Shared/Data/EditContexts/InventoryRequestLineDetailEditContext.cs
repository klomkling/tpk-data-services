using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class InventoryRequestLineDetailEditContext : EditContextBase<InventoryRequestLineDetail>
    {
        public InventoryRequestLineDetailEditContext() : base(null)
        {
        }

        public InventoryRequestLineDetailEditContext(InventoryRequestLineDetail dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            InventoryRequestLineId = DataItem.InventoryRequestLineId;
            ProductId = DataItem.ProductId;
            PackageTypeId = DataItem.PackageTypeId;
            StockroomId = DataItem.StockroomId;
            StockLocationId = DataItem.StockLocationId;
            LotNumber = DataItem.LotNumber;
            PackageNumber = DataItem.PackageNumber;
            PalletNo = DataItem.PalletNo;
            Quantity = DataItem.Quantity;
            ConfirmQuantity = DataItem.ConfirmQuantity;
            IsImported = DataItem.IsImported;
        }

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

        [NotMapped] public string ProductCode { get; set; }
        [NotMapped] public string StockroomName { get; set; }
        [NotMapped] public string StockLocationName { get; set; }
        [NotMapped] public string PackageTypeCode { get; set; }
    }
}