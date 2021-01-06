using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class ProductionOrderEditContext : EditContextBase<ProductionOrder>
    {
        private string _orderNumberDisplay;
        
        public ProductionOrderEditContext() : base(null)
        {
        }
        
        public ProductionOrderEditContext(ProductionOrder dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            OrderNumber = DataItem.OrderNumber;
            OrderDate = DataItem.OrderDate;
            DueDate = DataItem.DueDate;
            Status = DataItem.Status;
            StatusDate = DataItem.StatusDate;
            CompletedDate = DataItem.CompletedDate;
            LotNumber = DataItem.LotNumber;
            ProductId = DataItem.ProductId;
            Quantity = DataItem.Quantity;
            ReadyQuantity = DataItem.ReadyQuantity;
            Comment = DataItem.Comment;
        }
        
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string LotNumber { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReadyQuantity { get; set; }
        public string Comment { get; set; }
        
        [NotMapped]
        public string OrderNumberDisplay
        {
            get => $"{OrderNumber:000000}";
            set => _orderNumberDisplay = value;
        }
        
        [NotMapped] public string ProductName { get; set; }
        [NotMapped] public string ProductCode { get; set; }
    }
}