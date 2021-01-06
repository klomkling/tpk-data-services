using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class StockTransactionEditContext : EditContextBase<StockTransaction>
    {
        public StockTransactionEditContext() : base(null)
        {
        }

        public StockTransactionEditContext(StockTransaction dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            TransactionDate = DataItem.TransactionDate;
            TransactionType = DataItem.TransactionType;
            ProductId = DataItem.ProductId;
            StockroomId = DataItem.StockroomId;
            StockLocationId = DataItem.StockLocationId;
            PackageTypeId = DataItem.PackageTypeId;
            LotNumber = DataItem.LotNumber;
            PackageNumber = DataItem.PackageNumber;
            PalletNo = DataItem.PalletNo;
            PackingReference = DataItem.PackingReference;
            Quantity = DataItem.Quantity;
            Cost = DataItem.Cost;
            Balance = DataItem.Balance;
            IsAdjustStock = DataItem.IsAdjustStock;
            IsFragment = DataItem.IsFragment;
            Remark = DataItem.Remark;
        }

        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public int ProductId { get; set; }
        public int StockroomId { get; set; }
        public int StockLocationId { get; set; }
        public int? PackageTypeId { get; set; }
        public string LotNumber { get; set; }
        public int PackageNumber { get; set; }
        public string PalletNo { get; set; }
        public string PackingReference { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }
        public bool IsAdjustStock { get; set; }
        public bool IsFragment { get; set; }
        public string Remark { get; set; }

        [NotMapped] public decimal Balance { get; set; }
        [NotMapped]
        public string Status
        {
            get => IsAdjustStock ? "Adjust Stock" : "Normal";
            set => IsAdjustStock = value.Equals("Adjust Stock", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}