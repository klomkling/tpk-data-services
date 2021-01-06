using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class ProductionRequestEditContext : EditContextBase<ProductionRequest>
    {
        private string _requestNumberDisplay;
        
        public ProductionRequestEditContext() : base(null)
        {
        }
        
        public ProductionRequestEditContext(ProductionRequest dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            RequestNumber = DataItem.RequestNumber;
            RequestDate = DataItem.RequestDate;
            DueDate = DataItem.DueDate;
            Status = DataItem.Status;
            StatusDate = DataItem.StatusDate;
            CompletedDate = DataItem.CompletedDate;
            ProductId = DataItem.ProductId;
            Quantity = DataItem.Quantity;
            Comment = DataItem.Comment;
        }
        
        public int RequestNumber { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public string Comment { get; set; }
        
        [NotMapped]
        public string OrderNumberDisplay
        {
            get => $"{RequestNumber:000000}";
            set => _requestNumberDisplay = value;
        }

        [NotMapped] public string ProductName { get; set; }
        [NotMapped] public string ProductCode { get; set; }
    }
}