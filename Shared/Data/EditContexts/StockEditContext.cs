using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class StockEditContext : EditContextBase<Stock>
    {
        public StockEditContext() : base(null)
        {
        }

        public StockEditContext(Stock dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            ReceivedDate = DataItem.ReceivedDate;
            ProductId = DataItem.ProductId;
            StockroomId = DataItem.StockroomId;
            StockLocationId = DataItem.StockLocationId;
            PackageTypeId = DataItem.PackageTypeId;
            LotNumber = DataItem.LotNumber;
            PackageNumber = DataItem.PackageNumber;
            PalletNo = DataItem.PalletNo;
            PackingReference = DataItem.PackingReference;
            Quantity = DataItem.Quantity;
            UsedQuantity = DataItem.UsedQuantity;
            AvailableQuantity = DataItem.AvailableQuantity;
            Cost = DataItem.Cost;
            Balance = DataItem.Balance;
            
        }

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