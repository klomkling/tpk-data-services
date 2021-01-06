using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class InventoryRequestLineEditContext : EditContextBase<InventoryRequestLine>
    {
        private string _readyQuantityDisplay;

        public InventoryRequestLineEditContext() : base(null)
        {
        }

        public InventoryRequestLineEditContext(InventoryRequestLine dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            InventoryRequestId = DataItem.InventoryRequestId;
            SupplierProductId = DataItem.SupplierProductId;
            ProductId = DataItem.ProductId;
            Description = DataItem.Description;
            Quantity = DataItem.Quantity;
            ReadyQuantity = DataItem.ReadyQuantity;
            Status = DataItem.Status;
            StatusDate = DataItem.StatusDate;
            CompletedDate = DataItem.CompletedDate;
        }

        public int InventoryRequestId { get; set; }
        public int? SupplierProductId { get; set; }
        public int? ProductId { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReadyQuantity { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        [NotMapped] public DateTime? RequestDate => DataItem.CreatedAt;

        [NotMapped]
        public string ReadyQuantityDisplay
        {
            get => $"{ReadyQuantity:N}";
            set => _readyQuantityDisplay = value;
        }
    }
}