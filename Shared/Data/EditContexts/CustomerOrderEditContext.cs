using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class CustomerOrderEditContext : EditContextBase<CustomerOrder>
    {
        private string _amountDisplay;
        private string _grandTotalDisplay;
        private string _netAmountDisplay;
        private string _orderNumberDisplay;
        private string _vatAmountDisplay;

        public CustomerOrderEditContext() : base(null)
        {
        }

        public CustomerOrderEditContext(CustomerOrder dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            CustomerId = DataItem.CustomerId;
            OrderNumber = DataItem.OrderNumber;
            OrderDate = DataItem.OrderDate;
            DueDate = DataItem.DueDate;
            CustomerReference = DataItem.CustomerReference;
            OurReference = DataItem.OurReference;
            Status = DataItem.Status;
            StatusDate = DataItem.StatusDate;
            CompletedDate = DataItem.CompletedDate;
            Amount = DataItem.Amount;
            DiscountRate = DataItem.DiscountRate;
            DiscountAmount = DataItem.DiscountAmount;
            NetAmount = DataItem.NetAmount;
            VatRate = DataItem.VatRate;
            VatAmount = DataItem.VatAmount;
            GrandTotal = DataItem.GrandTotal;
            Comment = DataItem.Comment;
        }

        public int CustomerId { get; set; }
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public string CustomerReference { get; set; }
        public string OurReference { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public decimal Amount { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal VatRate { get; set; }
        public decimal VatAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public string Comment { get; set; }

        [NotMapped]
        public string OrderNumberDisplay
        {
            get => $"{OrderNumber:000000}";
            set => _orderNumberDisplay = value;
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
                VatAmount = NetAmount * VatRate / 100m;
                GrandTotal = NetAmount + VatAmount;
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
            get => $"{NetAmount:N}";
            set => _netAmountDisplay = value;
        }

        [NotMapped]
        public string VatAmountDisplay
        {
            get => $"{VatAmount:N}";
            set => _vatAmountDisplay = value;
        }

        [NotMapped]
        public string GrandTotalDisplay
        {
            get => $"{GrandTotal:N}";
            set => _grandTotalDisplay = value;
        }
    }
}