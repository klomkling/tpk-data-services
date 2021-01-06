using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class SupplierOrderLineEditContext : EditContextBase<SupplierOrderLine>
    {
        private string _amountDisplay;
        private string _netAmountDisplay;

        public SupplierOrderLineEditContext() : base(null)
        {
        }

        public SupplierOrderLineEditContext(SupplierOrderLine dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            SupplierOrderId = DataItem.SupplierOrderId;
            SupplierProductId = DataItem.SupplierProductId;
            ProductId = DataItem.ProductId;
            Description = DataItem.Description;
            Quantity = DataItem.Quantity;
            Price = DataItem.Price;
            Amount = DataItem.Amount;
            DiscountRate = DataItem.DiscountRate;
            DiscountAmount = DataItem.DiscountAmount;
            NetAmount = DataItem.NetAmount;
            Remark = DataItem.Remark;
        }

        public int SupplierOrderId { get; set; }
        public int SupplierProductId { get; set; }
        public int? ProductId { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string Remark { get; set; }

        [NotMapped]
        public decimal EditQuantity
        {
            get => Quantity;
            set
            {
                if (Equals(Quantity, value)) return;
                Quantity = value;
                Amount = value * Price;
                if (DiscountRate > 0m) DiscountAmount = Amount * DiscountRate / 100m;

                NetAmount = Amount - DiscountAmount;
            }
        }

        [NotMapped]
        public decimal EditPrice
        {
            get => Price;
            set
            {
                if (Equals(Price, value)) return;
                Price = value;
                Amount = value * Quantity;
                if (DiscountRate > 0m) DiscountAmount = Amount * DiscountRate / 100m;

                NetAmount = Amount - DiscountAmount;
            }
        }

        [NotMapped]
        public decimal EditDiscountRate
        {
            get => DiscountRate;
            set
            {
                if (Equals(DiscountRate, value)) return;
                DiscountRate = value;
                DiscountAmount = Amount * value / 100m;
                NetAmount = Amount - DiscountAmount;
            }
        }

        [NotMapped]
        public decimal EditDiscountAmount
        {
            get => DiscountAmount;
            set
            {
                if (Equals(DiscountAmount, value)) return;
                DiscountAmount = value;
                DiscountRate = 0m;
                NetAmount = Amount - DiscountAmount;
            }
        }

        [NotMapped]
        public string AmountDisplay
        {
            get => $"{Amount:N}";
            set => _amountDisplay = value;
        }

        [NotMapped]
        public string NetAmountDisplay
        {
            get => $"{NetAmount:N2}";
            set => _netAmountDisplay = value;
        }

        [NotMapped] public string ProductCode { get; set; }
    }
}